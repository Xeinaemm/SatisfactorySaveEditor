"Exporting buildings to file. This can take a really, really long time. A beep will play when it is finished."

Function CommandExists
{
	#Needs to be done by a function so the script can still print its error message
	Param ($command)
	try {if(Get-Command $command) {RETURN $true}}
	catch {RETURN $false}
}

if(CommandExists strings)
{
	if([System.IO.File]::Exists(".\FactoryGame-Windows.pak"))
	{
		#command that does not save to file, instead printing it to the console
		#strings -n 30 .\FactoryGame-Windows.pak | Select-String -Pattern "^BlueprintGeneratedClass'/Game/FactoryGame/Schematics" | % { "$_".replace("BlueprintGeneratedClass'", "") } | % { "$_".replace("'", "")}

		strings -n 30 .\FactoryGame-Windows.pak | Select-String -Pattern "^BlueprintGeneratedClass'/Game/FactoryGame/Buildable/Factory" | ForEach-Object { "$_".replace("BlueprintGeneratedClass'", "") } | ForEach-Object { "$_".replace("'", "")} | Tee-Object -FilePath .\Buildables.txt
		[console]::beep(500,125)
		"Done."
	}
	else
	{
		Write-Host "Couldn't find FactoryGame-Windows.pak\nAre you sure you copied this script into your \SatisfactoryEarlyAccess\FactoryGame\Content\Paks folder?" -ForegroundColor Red
	}
}
else
{
	Write-Host "You don't have an implementation of 'strings' installed. Please install one, such as the one linked here: `nhttps://docs.microsoft.com/en-us/sysinternals/downloads/strings`nYou can install this implementation by putting the exe in your system32 folder, or somewhere else added to PATH" -ForegroundColor Red
}

pause

