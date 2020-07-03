# Load WinSCP .NET assembly
CMD /c PAUSE
Add-Type -Path "$PSScriptRoot\WinSCPnet.dll"

function FileTransferProgress {

param($e)

Write-Progress `

-Activity "Uploading" -Status ("{0:P0} complete:" -f $e.OverallProgress) `

-PercentComplete ($e.OverallProgress * 100)

Write-Progress `

-Id 1 -Activity $e.FileName -Status ("{0:P0} complete:" -f $e.FileProgress) `

-PercentComplete ($e.FileProgress * 100)}

# Set up session options

$sessionOptions = New-Object WinSCP.SessionOptions -Property @{
Protocol              = [WinSCP.Protocol]::Sftp
HostName              = "192.168.0.10"
UserName              = "pi"
Password              = "qawsed"
SshHostKeyFingerprint = "ssh-ed25519 255 adEE1YiHJUEcL3Qw57CXGLs20hRJFP/ArlftWYgA/aA="

}

$session = New-Object WinSCP.Session

try {

# Will continuously report progress of transfer

$session.add_FileTransferProgress( { FileTransferProgress($_) } )

# Connect

$session.Open($sessionOptions)
CMD /c PAUSE
try {$session.ExecuteCommand("killall DAPISmartHome_BackendServer").Check();

}

catch {Write-Host 'didnt kill DAPISmartHome_BackendServer because it wasnt running '

}

Start-Process dotnet -ArgumentList 'publish -r linux-arm' -Wait -NoNewWindow -WorkingDirectory $PSScriptRoot

$result = $session.PutFiles("$PSScriptRoot\bin\Debug\netcoreapp3.0\linux-arm\publish\*", "Desktop/ontobike/").Check();

Write-Host $result

$session.ExecuteCommand("chown pi /home/pi/Desktop/DAPISmartHome_BackendServer -R").Check();

$session.ExecuteCommand("chmod 777 /home/pi/Desktop/DAPISmartHome_BackendServer -R").Check();

$session.ExecuteCommand("Desktop/DAPISmartHome_BackendServer/DAPISmartHome_BackendServer").Check();

}

finally {
CMD /c PAUSE
$session.Dispose()

}