param(
	[string]$project = ".\src\Owin.Security.Saml\Owin.Security.Saml.csproj",
	[string]$solution = ".\src\SAML2.sln",
	[string]$assemblyInfoFile = ".\src\Owin.Security.Saml\Properties\AssemblyInfo.cs",
	[string]$nuspecFile = ".\src\Owin.Security.Saml\Owin.Security.Saml.nuspec"
)

# Tool locations
$nuget = ".\tools\NuGet.exe"
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

# RegEx strings
$assemblyVersionPattern = '^\[assembly: AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)\]$'
$fileVersionPattern = '^\[assembly: AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)\]$'

# Get version for package
$version = Read-Host 'Version (major.minor)'
if (($version -eq '/?') -or ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}"))
{
	Write-Host "Incorrect format for version"
	return;
}

# Get release notes for package
$releaseNotes = Read-Host 'Release notes'

# Set new assembly and file version strings
$zeroPad = ""
$starPad = ".*"
switch ($version.Split(".").Length)
{
	4 { $starpad = "" }
	3 { $zeroPad = ".0" }
	2 { $zeroPad = ".0.0" }
}

$assemblyVersion = "[assembly: AssemblyVersion(`"$version$zeroPad`")]"
$fileVersion = "[assembly: AssemblyFileVersion(`"$version$zeroPad`")]"

# Change version number
$assemblyInfo = Get-Content -Encoding UTF8 $assemblyInfoFile
$oldAssemblyVersion = $assemblyInfo -match $assemblyVersionPattern
$oldFileVersion = $assemblyInfo -match  $fileVersionPattern

$assemblyInfo = $assemblyInfo -replace $assemblyVersionPattern, $assemblyVersion
$assemblyInfo = $assemblyInfo -replace $fileVersionPattern, $fileVersion

Set-Content -Encoding UTF8 $assemblyInfoFile $assemblyInfo

# Change release notes for package
$nuspec = Get-Content $nuspecFile
$nuspec = $nuspec -replace "<releaseNotes></releaseNotes>", "<releaseNotes>$releaseNotes</releaseNotes>"
Set-Content -Encoding UTF8 $nuspecFile $nuspec

# Clean
Invoke-Expression "$msbuild $solution /p:Configuration=Debug /p:Platform=`"Any CPU`" /t:Clean"
Invoke-Expression "$msbuild $solution /p:Configuration=Release /p:Platform=`"Any CPU`" /t:Clean"

# Optional: Build
# Invoke-Expression "$msbuild $solution /p:Configuration=Release /p:Platform=`"Any CPU`" /t:Build"

# Optional: Run unit tests
# Invoke-Expression ".\src\packages\NUnit.Runners\tools\nunit.exe"

# Run nuget
# Invoke-Expression "$nuget pack $project"
Invoke-Expression "$nuget pack $project -Build -Properties Configuration=Release"

# Revert version number
$assemblyInfo = $assemblyInfo -replace $assemblyVersionPattern, $oldAssemblyVersion
$assemblyInfo = $assemblyInfo -replace $fileVersionPattern, $oldFileVersion
Set-Content -Encoding UTF8 $assemblyInfoFile $assemblyInfo

# Revert NuSpec file
$nuspec = $nuspec -replace "<releaseNotes>$releaseNotes</releaseNotes>","<releaseNotes></releaseNotes>"
Set-Content -Encoding UTF8 $nuspecFile $nuspec
