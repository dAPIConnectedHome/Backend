# Load WinSCP .NET assembly
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

try {$session.ExecuteCommand("killall TEst").Check();

}

catch {Write-Host 'didnt kill TEst because it wasnt running '

}

Start-Process dotnet -ArgumentList "publish -r linux-arm" -Wait -NoNewWindow -WorkingDirectory $PSScriptRoot

Write-Host "dotnet published"
CMD /c PAUSE

#scp copy
$result = $session.PutFiles("$PSScriptRoot\bin\Debug\netcoreapp3.1\linux-arm\publish\*", "Desktop/TEst/").Check();
Write-Host "result received"


Write-Host $result

$session.ExecuteCommand("chown pi /home/pi/Desktop/TEst -R").Check();
Write-Host "chown done"

$session.ExecuteCommand("chmod 777 /home/pi/Desktop/TEst -R").Check();
Write-Host "chmod done"

$session.ExecuteCommand("dotnet run Desktop/TEst/TEst").Check();
Write-Host "Desktop done???"

}

catch{

Write-Host "Error Occured"
CMD /c PAUSE

}

finally {
$session.Dispose()

}