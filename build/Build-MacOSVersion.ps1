# TODO: Build the MacOS version

Write-Host "This script builds the MacOS version of the application."

$projectName = $ENV:STRAVAIG_PROJECT;
$projectPath = "./src/$projectName/$projectName.csproj";
$appIconSet = "./src/$projectName/MacOS/icon.iconset";
$infoPListPath = "./src/$projectName/MacOS/Info.plist";

$outputDir = "./out";
$programDir = "$outputDir/Stravaig Conn Officer.app";
$contentsDir = "$programDir/Contents";
$codeSignatureDir = "$contentsDir/_CodeSignature";
$codeResourcesDir = "$codeSignatureDir/CodeResources";
$macOSDir = "$contentsDir/MacOS";
$resourcesDir = "$contentsDir/Resources";
$iconResourcePath = "$resourceDir/ConnOfficerAppIcon.icns";

Write-Host ""
Write-Host "Creating application directory structure";
$null = New-Item -ItemType Directory -Force -Path $codeResourcesDir;
$null = New-Item -ItemType Directory -Force -Path $MacOSDir;
$null = New-Item -ItemType Directory -Force -Path $resourcesDir;

Write-Host ""
Write-Host "Converting $appIconSet to $iconResourcePath";
iconutil -c icns --output "$iconResourcePath" "$appIconSet"


Write-Host ""
Write-Host "Publishing application";
dotnet publish $projectPath --runtime osx-x64 --configuration Release -p:UseAppHost=true --output $macOSDir --self-contained true -p:PublishSingleFile=true
