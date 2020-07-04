Add-Type -Path "$PSScriptRoot\WinSCPnet.dll"


Param(
[paramter(Mandatory=$true)][object][ValidateNotNullOrEmpty()] $session
)
Param(
[paramter(Mandatory=$true)][string[]][ValidateNotNullOrEmpty()] $publishlocation
)
Param(
[paramter(Mandatory=$true)][string[]][ValidateNotNullOrEmpty()] $targetlocation
)

$result = $session.PutFiles("publishlocation", "targetlocation").Check();
write-host $result