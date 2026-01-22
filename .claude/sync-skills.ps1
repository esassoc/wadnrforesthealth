# sync-skills.ps1
# Syncs Claude skills from this project to other projects
#
# Usage:
#   .\sync-skills.ps1              # Sync to all configured projects
#   .\sync-skills.ps1 -WhatIf      # Preview what would be copied
#   .\sync-skills.ps1 -Verbose     # Show detailed output

param(
    [switch]$WhatIf,
    [switch]$Verbose
)

$sourceDir = Join-Path $PSScriptRoot "commands"

# Add destination projects here
$destinations = @(
    # "C:\git\sitkatech\other-project\.claude\commands"
    # "C:\git\sitkatech\another-project\.claude\commands"
)

if ($destinations.Count -eq 0) {
    Write-Host "No destination projects configured." -ForegroundColor Yellow
    Write-Host "Edit sync-skills.ps1 and add paths to the `$destinations array." -ForegroundColor Yellow
    exit 0
}

$skillFiles = Get-ChildItem -Path $sourceDir -Filter "*.md"

Write-Host "Syncing $($skillFiles.Count) skills from WADNR..." -ForegroundColor Cyan

foreach ($dest in $destinations) {
    if (-not (Test-Path $dest)) {
        Write-Host "  Creating: $dest" -ForegroundColor Yellow
        if (-not $WhatIf) {
            New-Item -ItemType Directory -Path $dest -Force | Out-Null
        }
    }

    foreach ($file in $skillFiles) {
        $destFile = Join-Path $dest $file.Name
        if ($Verbose) {
            Write-Host "  $($file.Name) -> $dest" -ForegroundColor Gray
        }
        if (-not $WhatIf) {
            Copy-Item $file.FullName $destFile -Force
        }
    }

    Write-Host "  Synced to: $dest" -ForegroundColor Green
}

if ($WhatIf) {
    Write-Host "`n(WhatIf mode - no files were copied)" -ForegroundColor Yellow
}

Write-Host "`nDone! Remember to update Project Variables in each project's CLAUDE.md" -ForegroundColor Cyan
