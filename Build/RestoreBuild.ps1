

"Restore WADNRForestHealthTracker"
& "$PSScriptRoot\DatabaseRestore.ps1"  -iniFile "./build.ini"

"Build WADNRForestHealthTracker"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
