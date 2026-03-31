
"Download WADNR"
& "$PSScriptRoot\DatabaseDownload.ps1" -iniFile "./build.ini" -secretsIniFile "./secrets.ini"

"Restore WADNR"
& "$PSScriptRoot\DatabaseRestore.ps1" -iniFile "./build.ini"
