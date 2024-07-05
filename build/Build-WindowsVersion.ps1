# TODO: Build the windows version

Write-Host "This script builds the windows version of Stravaig Conn Officer"

$projectName = $ENV:STRAVAIG_PROJECT;
$projectPath = "./src/$projectName/$projectName.csproj"

$baseDir = "./out";

dotnet publish $projectPath --runtime win-x64 --configuration Release --output $baseDir --self-contained true -p:PublishSingleFile=true
