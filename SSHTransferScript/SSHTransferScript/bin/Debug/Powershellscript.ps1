$session = $args[0]
$result = $session.PutFiles("$PSScriptRoot\bin\Debug\netcoreapp3.1\linux-arm\publish\*", "Desktop/TEst/").Check();
write-host $param1