$Command = "C:\Source\DatabaseExtract\DatabaseExtract\bin\Debug\net8.0\DatabaseExtract.exe"
$Params = "C:\DatabaseTest\RunFromPS.csv"

$Params = $Parms.Split(" ")
& "$Command" $Params