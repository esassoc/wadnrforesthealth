

"Restore WADNR"
& "$PSScriptRoot\DatabaseRestore.ps1"  -iniFile "./build.ini"

"Build WADNR"
& "$PSScriptRoot\DatabaseBuild.ps1" -iniFile "./build.ini"
