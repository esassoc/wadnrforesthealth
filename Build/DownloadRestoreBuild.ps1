
"Download WADNRForestHealthTracker"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile "./build.ini" -secretsIniFile "./secrets.ini"

"Restore WADNRForestHealthTracker"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile "./build.ini"

"Build WADNRForestHealthTracker"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
