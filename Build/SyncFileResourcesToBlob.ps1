# Sync FileResource binary data from the restored legacy database to Azure Blob Storage.
# Reads the FileResourceData column from dbo.FileResource and uploads using Az.Storage module.
# After each successful batch, updates InBlobStorage = 1 and sets ContentLength.
#
# The legacy on-prem database (WADNRForestHealthDB) stores file data inline in
# dbo.FileResource.FileResourceData. The new Azure database (WADNRDB) expects that
# data to live in blob storage and does not have the FileResourceData column.
#
# Intended workflow:
#   1. Download + restore database backup from production (DatabaseDownload.ps1 / DatabaseRestore.ps1)
#      This restores WADNRForestHealthDB which still has the FileResourceData column.
#   2. Run this script against WADNRForestHealthDB to upload any new files to blob storage.
#      Use -StartAfterID with the last FileResourceID from the previous run.
#   3. Run DatabaseBuild.ps1 which deploys the .sqlproj schema (drops FileResourceData column, etc.)
#   4. Continue with Scaffold.ps1 and the rest of the pipeline.
#
# Prerequisites: Az.Storage PowerShell module (Install-Module Az.Storage).
#
# Usage:
#   # Dry run to see what would be uploaded:
#   .\SyncFileResourcesToBlob.ps1 -ServerInstance ".\" -Database "WADNRForestHealthDB" -DryRun
#
#   # First run (all records, creates wadnr-file-resource container if needed):
#   .\SyncFileResourcesToBlob.ps1 -ServerInstance ".\" -Database "WADNRForestHealthDB" -BlobConnectionString "DefaultEndpointsProtocol=https;AccountName=psinfoappprod;AccountKey=...;EndpointSuffix=core.windows.net"
#
#   # Subsequent runs (only new records since last sync):
#   .\SyncFileResourcesToBlob.ps1 -ServerInstance ".\" -Database "WADNRForestHealthDB" -StartAfterID 14895 -BlobConnectionString "DefaultEndpointsProtocol=..."
#
#   # Upload to prod storage, then copy container to dev storage for local development:
#   .\SyncFileResourcesToBlob.ps1 -ServerInstance ".\" -Database "WADNRForestHealthDB" -BlobConnectionString "DefaultEndpointsProtocol=https;AccountName=psinfoappprod;AccountKey=...;EndpointSuffix=core.windows.net" -CopyToBlobConnectionString "DefaultEndpointsProtocol=https;AccountName=wadnrappdev;AccountKey=...;EndpointSuffix=core.windows.net"

param(
    [string]$ConnectionString,
    [string]$ServerInstance,
    [string]$Database,
    [int]$StartAfterID = 0,
    [string]$BlobConnectionString = "",
    [string]$ContainerName = "wadnr-file-resource",
    [string]$CopyToBlobConnectionString = "",
    [string]$CopyToContainerName = "",
    [switch]$DryRun
)

if (-not $ConnectionString) {
    if (-not $ServerInstance -or -not $Database) {
        Write-Error "Provide either -ConnectionString or both -ServerInstance and -Database"
        exit 1
    }
    $ConnectionString = "Server=$ServerInstance;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True;"
}

if (-not $BlobConnectionString -and -not $DryRun) {
    Write-Error "Provide -BlobConnectionString (Azure Storage connection string) or use -DryRun"
    exit 1
}

# Default CopyToContainerName to same as ContainerName
if (-not $CopyToContainerName) {
    $CopyToContainerName = $ContainerName
}

# Set up Azure Storage context (in-process, reused for all uploads - much faster than az CLI)
$storageContext = $null
if (-not $DryRun) {
    Import-Module Az.Storage -ErrorAction Stop
    $storageContext = New-AzStorageContext -ConnectionString $BlobConnectionString
    Write-Host "Azure Storage context created for account '$($storageContext.StorageAccountName)'." -ForegroundColor DarkGray

    # Ensure blob container exists (no-op if it already exists)
    Write-Host "Ensuring container '$ContainerName' exists..." -ForegroundColor DarkGray
    New-AzStorageContainer -Name $ContainerName -Context $storageContext -ErrorAction SilentlyContinue | Out-Null
}

# Connect to SQL Server
$conn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
$conn.Open()
Write-Host "Connected to database." -ForegroundColor Green

# Check if FileResourceData column exists (it gets dropped by DatabaseBuild.ps1 / .sqlproj)
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FileResource' AND TABLE_SCHEMA = 'dbo' AND COLUMN_NAME = 'FileResourceData'"
$columnExists = $cmd.ExecuteScalar()
if ($columnExists -eq 0) {
    Write-Host "FileResourceData column does not exist on dbo.FileResource." -ForegroundColor Yellow
    Write-Host "This script must run against a freshly restored database backup (before DatabaseBuild.ps1 drops the column)." -ForegroundColor Yellow
    $conn.Close()
    exit 0
}

# Ensure InBlobStorage and ContentLength columns exist (the restored legacy backup may not have them)
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FileResource' AND TABLE_SCHEMA = 'dbo' AND COLUMN_NAME = 'InBlobStorage'"
if ($cmd.ExecuteScalar() -eq 0) {
    Write-Host "Adding InBlobStorage column to dbo.FileResource..." -ForegroundColor DarkGray
    $alterCmd = $conn.CreateCommand()
    $alterCmd.CommandText = "ALTER TABLE dbo.FileResource ADD InBlobStorage bit NOT NULL DEFAULT(0)"
    $alterCmd.ExecuteNonQuery() | Out-Null
}

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FileResource' AND TABLE_SCHEMA = 'dbo' AND COLUMN_NAME = 'ContentLength'"
if ($cmd.ExecuteScalar() -eq 0) {
    Write-Host "Adding ContentLength column to dbo.FileResource..." -ForegroundColor DarkGray
    $alterCmd = $conn.CreateCommand()
    $alterCmd.CommandText = "ALTER TABLE dbo.FileResource ADD ContentLength bigint NULL"
    $alterCmd.ExecuteNonQuery() | Out-Null
}

# Count records to sync
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT COUNT(*)
FROM dbo.FileResource
WHERE FileResourceData IS NOT NULL
  AND FileResourceID > $StartAfterID
"@
$totalCount = $cmd.ExecuteScalar()

if ($StartAfterID -gt 0) {
    Write-Host "Processing FileResourceID > $StartAfterID ($totalCount record(s))." -ForegroundColor Cyan
} else {
    Write-Host "Processing ALL $totalCount FileResource record(s) with data." -ForegroundColor Cyan
}

if ($totalCount -eq 0) {
    Write-Host "Nothing to sync." -ForegroundColor Green
    $conn.Close()
} else {
    # Create temp directory for file staging
    $tempDir = Join-Path $env:TEMP "FileResourceSync"
    if (-not (Test-Path $tempDir)) {
        New-Item -ItemType Directory -Path $tempDir | Out-Null
    }

    # Process in batches
    $batchSize = 50
    $lastID = $StartAfterID
    $uploaded = 0
    $errors = 0
    $totalBytes = [long]0
    $processed = 0
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

    while ($processed -lt $totalCount) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = @"
SELECT TOP $batchSize
       FileResourceID, FileResourceGUID,
       OriginalBaseFilename, OriginalFileExtension,
       FileResourceData, DATALENGTH(FileResourceData) AS DataSize
FROM dbo.FileResource
WHERE FileResourceData IS NOT NULL
  AND FileResourceID > $lastID
ORDER BY FileResourceID
"@
        $cmd.CommandTimeout = 600

        $reader = $cmd.ExecuteReader()
        $batchHadRows = $false
        $idsToUpdate = @()
        $idSizeMap = @{}
        while ($reader.Read()) {
            $batchHadRows = $true
            $id = $reader["FileResourceID"]
            $guid = $reader["FileResourceGUID"].ToString().ToLower()
            $name = $reader["OriginalBaseFilename"]
            $ext = $reader["OriginalFileExtension"]
            $dataSize = [long]$reader["DataSize"]
            $sizeMB = [Math]::Round($dataSize / 1MB, 2)
            $lastID = $id
            $processed++

            if ($DryRun) {
                Write-Host "  [$processed/$totalCount] ID=$id WOULD UPLOAD $name.$ext ($guid, $sizeMB MB)" -ForegroundColor Yellow
                $uploaded++
                $totalBytes += $dataSize
                continue
            }

            # Write binary data to temp file, upload via Az.Storage, then delete
            try {
                $data = [byte[]]$reader["FileResourceData"]
                $tempFile = Join-Path $tempDir $guid
                [System.IO.File]::WriteAllBytes($tempFile, $data)

                Set-AzStorageBlobContent -Container $ContainerName -Blob $guid -File $tempFile -Context $storageContext -Force | Out-Null

                Remove-Item $tempFile -Force -ErrorAction SilentlyContinue

                Write-Host "  [$processed/$totalCount] ID=$id UPLOADED $name.$ext ($guid, $sizeMB MB)" -ForegroundColor Green
                $uploaded++
                $totalBytes += $dataSize
                $idsToUpdate += $id
                $idSizeMap[$id] = $dataSize
            }
            catch {
                Write-Host "  [$processed/$totalCount] ID=$id ERROR $name.$ext ($guid): $_" -ForegroundColor Red
                $errors++
                Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
            }
        }
        $reader.Close()

        # Update InBlobStorage and ContentLength for successfully uploaded records
        if (-not $DryRun -and $idsToUpdate.Count -gt 0) {
            foreach ($updateId in $idsToUpdate) {
                $updateCmd = $conn.CreateCommand()
                $updateCmd.CommandText = "UPDATE dbo.FileResource SET InBlobStorage = 1, ContentLength = $($idSizeMap[$updateId]) WHERE FileResourceID = $updateId"
                $updateCmd.ExecuteNonQuery() | Out-Null
            }
            Write-Host "  Updated InBlobStorage + ContentLength for $($idsToUpdate.Count) record(s)." -ForegroundColor DarkGreen
        }

        if (-not $batchHadRows) { break }
    }

    $conn.Close()
    $stopwatch.Stop()

    # Clean up temp directory
    Remove-Item $tempDir -Force -Recurse -ErrorAction SilentlyContinue

    $totalMB = [Math]::Round($totalBytes / 1MB, 2)
    $newLastID = $lastID
    $elapsed = $stopwatch.Elapsed.ToString("hh\:mm\:ss")

    Write-Host "`n================================" -ForegroundColor Cyan
    if ($DryRun) {
        Write-Host "DRY RUN complete (no uploads performed)" -ForegroundColor Yellow
    } else {
        Write-Host "Sync complete!" -ForegroundColor Green
    }
    Write-Host "Records processed: $processed"
    Write-Host "Uploaded: $uploaded ($totalMB MB)"
    if ($errors -gt 0) {
        Write-Host "Errors: $errors" -ForegroundColor Red
    }
    Write-Host "Elapsed: $elapsed"
    if ($processed -gt 0) {
        Write-Host "Last FileResourceID processed: $newLastID" -ForegroundColor White
        Write-Host "  (Use -StartAfterID $newLastID next time)" -ForegroundColor DarkYellow
    }
    Write-Host "================================" -ForegroundColor Cyan
} # end if ($totalCount -gt 0)

# ── Optional: Copy container to dev/target storage account ──
if ($CopyToBlobConnectionString -and -not $DryRun) {
    Write-Host "`nCopying container to target storage account..." -ForegroundColor Cyan

    $dstContext = New-AzStorageContext -ConnectionString $CopyToBlobConnectionString
    Write-Host "  Source:      $($storageContext.StorageAccountName)/$ContainerName" -ForegroundColor DarkGray
    Write-Host "  Destination: $($dstContext.StorageAccountName)/$CopyToContainerName" -ForegroundColor DarkGray

    # Ensure destination container exists
    New-AzStorageContainer -Name $CopyToContainerName -Context $dstContext -ErrorAction SilentlyContinue | Out-Null

    # List all source blobs and copy any that are missing or different size in destination
    $srcBlobs = Get-AzStorageBlob -Container $ContainerName -Context $storageContext
    $dstBlobs = Get-AzStorageBlob -Container $CopyToContainerName -Context $dstContext
    $dstBlobMap = @{}
    foreach ($b in $dstBlobs) {
        $dstBlobMap[$b.Name] = $b.Length
    }

    $toCopy = @()
    foreach ($b in $srcBlobs) {
        if (-not $dstBlobMap.ContainsKey($b.Name) -or $dstBlobMap[$b.Name] -ne $b.Length) {
            $toCopy += $b
        }
    }

    Write-Host "  Source blobs: $($srcBlobs.Count), Destination blobs: $($dstBlobs.Count), To copy: $($toCopy.Count)" -ForegroundColor DarkGray

    if ($toCopy.Count -eq 0) {
        Write-Host "  Destination is already up to date." -ForegroundColor Green
    } else {
        $copyStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        $copied = 0
        $copyErrors = 0
        foreach ($blob in $toCopy) {
            try {
                Start-AzStorageBlobCopy -SrcContainer $ContainerName -SrcBlob $blob.Name -DestContainer $CopyToContainerName -DestBlob $blob.Name -Context $storageContext -DestContext $dstContext -Force | Out-Null
                $copied++
                if ($copied % 100 -eq 0) {
                    Write-Host "  Copied $copied / $($toCopy.Count)..." -ForegroundColor DarkGray
                }
            } catch {
                Write-Host "  ERROR copying $($blob.Name): $_" -ForegroundColor Red
                $copyErrors++
            }
        }
        $copyStopwatch.Stop()
        Write-Host "  Copied $copied blob(s) in $($copyStopwatch.Elapsed.ToString('hh\:mm\:ss')). Errors: $copyErrors" -ForegroundColor $(if ($copyErrors -gt 0) { 'Yellow' } else { 'Green' })
    }

    Write-Host "Container copy complete." -ForegroundColor Green
}
