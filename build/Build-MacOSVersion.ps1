# TODO: Build the MacOS version

Write-Host "This script builds the MacOS version of the application."

$projectName = $ENV:STRAVAIG_PROJECT;
$projectPath = "./src/$projectName/$projectName.csproj"

$baseDir = "./out";
$programDir = "$baseDir/Stravaig Conn Officer.app";
$contentsDir = "$programDir/Contents";
$codeSignatureDir = "$contentsDir/_CodeSignature";
$codeResourcesDir = "$codeSignatureDir/CodeResources";

New-Item -ItemType Directory -Force -Path $codeResourcesDir;

$macOSDir = "$contentsDir/MacOS";
New-Item -ItemType Directory -Force -Path $MacOSDir;

$resourcesDir = "$contentsDir/Resources";
New-Item -ItemType Directory -Force -Path $resourcesDir;

dotnet publish $projectPath --runtime osx-x64 --configuration Release -p:UseAppHost=true --output $macOSDir --self-contained true -p:PublishSingleFile=true

