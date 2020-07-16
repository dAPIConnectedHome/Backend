Param
(
	[parameter(Mandatory=$true)]
	[string[]]
	[ValidateNotNullOrEmpty()]
	$SlnPath
)

Start-Process dotnet -ArgumentList "publish -r linux-arm" -Wait -NoNewWindow -WorkingDirectory "$SlnPath"

