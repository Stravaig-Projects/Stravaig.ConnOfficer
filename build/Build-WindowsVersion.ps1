function Log($message)
{
    Write-Host ""
    Write-Host("[$((Get-Date).ToString("yyyy-MM-dd HH:mm:ss.fff zzz"))] => $message");
}

Log "This script builds the windows version of Stravaig Conn Officer"

$repoRoot = [System.IO.Path]::GetFullPath("$PSScriptRoot/..");
$projectName = "Stravaig.ConnOfficer"
$projectPath = "$repoRoot/src/$projectName/$projectName.csproj"

$workingDir = Get-Location;
$outputDir = "$workingDir/out";
if (Test-Path $outputDir)
{
    Log("Cleaning up the output directory")
    Remove-Item $outputDir -Recurse -Force
}

dotnet publish $projectPath --runtime win-x64 --configuration Release --output $outputDir --self-contained true -p:PublishSingleFile=true

Remove-Item "$outputDir/*.pdb" -Force
Remove-Item "$outputDir/*.xml" -Force
