function Log($message)
{
    Write-Host ""
    Write-Host("[$((Get-Date).ToString("yyyy-MM-dd HH:mm:ss.fff zzz"))] => $message");
}

Log "This script builds the MacOS version of the application."

$projectName = "Stravaig.ConnOfficer";
$repoRoot = [System.IO.Path]::GetFullPath("$PSScriptRoot/..");
$projectPath = "$repoRoot/src/$projectName/$projectName.csproj";
$appIconSet = "$PSScriptRoot/MacOS/icon.iconset";
$infoPListPath = "$PSScriptRoot/MacOS/Info.plist";

$workingDir = Get-Location;
$outputDir = "$workingDir/out";
if (Test-Path $outputDir)
{
    Log("Cleaning up the output directory")
    Remove-Item $outputDir -Recurse -Force
}


$programDir = "$outputDir/Stravaig Conn Officer.app";
$contentsDir = "$programDir/Contents";
$codeSignatureDir = "$contentsDir/_CodeSignature";
$codeResourcesDir = "$codeSignatureDir/CodeResources";
$macOSDir = "$contentsDir/MacOS";
$resourcesDir = "$contentsDir/Resources";
$iconResourcePath = "$resourcesDir/ConnOfficerAppIcon.icns";

Log "Creating application directory structure";
$null = New-Item -ItemType Directory -Force -Path $codeResourcesDir;
$null = New-Item -ItemType Directory -Force -Path $MacOSDir;
$null = New-Item -ItemType Directory -Force -Path $resourcesDir;

Log "Building app icon. Converting`nFrom $appIconSet`n  To $iconResourcePath";
iconutil -c icns --output "$iconResourcePath" "$appIconSet"
if ($LASTEXITCODE -ne 0)
{
    Log "Failed to convert the iconset to an icns file. Exit code $LASTEXITCODE"
    Exit $LASTEXITCODE
}

Log "Publishing application`nFrom $projectPath`n  To $macOSDir";
dotnet publish $projectPath --runtime osx-x64 --configuration Release -p:UseAppHost=true --output $macOSDir --self-contained true -p:PublishSingleFile=true
if ($LASTEXITCODE -ne 0)
{
    Log "Failed to publish the application. Exit code $LASTEXITCODE"
    Exit $LASTEXITCODE
}

Log "Publishing Info.plist file";
Copy-Item -Path $infoPListPath -Destination $contentsDir;
