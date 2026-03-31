# sync-skills.ps1
# Syncs Claude rules and skills from this project to other projects
#
# Usage:
#   .\sync-skills.ps1              # Sync to all configured projects
#   .\sync-skills.ps1 -WhatIf      # Preview what would be copied
#   .\sync-skills.ps1 -Verbose     # Show detailed output

param(
    [switch]$WhatIf,
    [switch]$Verbose
)

$rulesDir = Join-Path $PSScriptRoot "rules"
$skillsDir = Join-Path $PSScriptRoot "skills"

# Add destination projects here
$destinations = @(
    # "C:\git\sitkatech\other-project\.claude"
    # "C:\git\sitkatech\another-project\.claude"
)

if ($destinations.Count -eq 0) {
    Write-Host "No destination projects configured." -ForegroundColor Yellow
    Write-Host "Edit sync-skills.ps1 and add paths to the `$destinations array." -ForegroundColor Yellow
    exit 0
}

# Collect rules (flat .md files)
$ruleFiles = Get-ChildItem -Path $rulesDir -Filter "*.md"

# Collect skills (SKILL.md in subdirectories)
$skillFiles = Get-ChildItem -Path $skillsDir -Recurse -Filter "SKILL.md"

$totalFiles = $ruleFiles.Count + $skillFiles.Count
Write-Host "Syncing $totalFiles files ($($ruleFiles.Count) rules, $($skillFiles.Count) skills) from WADNR..." -ForegroundColor Cyan

foreach ($dest in $destinations) {
    $destRules = Join-Path $dest "rules"
    $destSkills = Join-Path $dest "skills"

    # Sync rules
    if (-not (Test-Path $destRules)) {
        Write-Host "  Creating: $destRules" -ForegroundColor Yellow
        if (-not $WhatIf) {
            New-Item -ItemType Directory -Path $destRules -Force | Out-Null
        }
    }

    foreach ($file in $ruleFiles) {
        $destFile = Join-Path $destRules $file.Name
        if ($Verbose) {
            Write-Host "  rules/$($file.Name) -> $destRules" -ForegroundColor Gray
        }
        if (-not $WhatIf) {
            Copy-Item $file.FullName $destFile -Force
        }
    }

    # Sync skills (preserve subdirectory structure)
    foreach ($file in $skillFiles) {
        $skillName = $file.Directory.Name
        $destSkillDir = Join-Path $destSkills $skillName

        if (-not (Test-Path $destSkillDir)) {
            if ($Verbose) {
                Write-Host "  Creating: $destSkillDir" -ForegroundColor Yellow
            }
            if (-not $WhatIf) {
                New-Item -ItemType Directory -Path $destSkillDir -Force | Out-Null
            }
        }

        $destFile = Join-Path $destSkillDir "SKILL.md"
        if ($Verbose) {
            Write-Host "  skills/$skillName/SKILL.md -> $destSkillDir" -ForegroundColor Gray
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
