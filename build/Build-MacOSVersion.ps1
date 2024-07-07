# Expected Environment Variables:
# STRAVAIG_APP_VERSION

function Log($message)
{
    Write-Host ""
    Write-Host("[$((Get-Date).ToString("yyyy-MM-dd HH:mm:ss.fff zzz"))] => $message");
}

function UpdateInfoPList($File, $Key, $Type, $Value)
{
    # Read the info.plist file
    [xml]$plist = Get-Content -Path $File;

    $childNodes = $plist.plist.dict.ChildNodes;
    $nodeCount = $childNodes.Count
    for($i = 0; $i -lt $nodeCount; $i += 1)
    {
        $node = $childNodes[$i];
        if ($node.Name -eq "key" -and $node.InnerText -eq $Key)
        {
            Log "Updating $Key to $Value"
            $valueNode = $childNodes[$i + 1];

            # Create a new node
            $newNode = $plist.CreateElement($Type)
            $newNode.InnerText = $Value;

            # Replace the existing node with the new one
            $null = $plist.plist.dict.RemoveChild($valueNode);
            $null = $plist.plist.dict.InsertBefore($newNode, $childNodes[$i + 1]);
        }
    }

    # Save the modified plist back to the file with indentation
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true

    $writer = [System.Xml.XmlWriter]::Create($File, $settings)
    try
    {
        $plist.Save($writer)
    }
    finally
    {
        $writer.Flush()
        $writer.Close()
    }
}

Log "This script builds the MacOS version of the application."

$stravaigAppVersion = $ENV:STRAVAIG_APP_VERSION
if ($null -eq $stravaigAppVersion)
{
    Log "Cannot find environment variable STRAVAIG_APP_VERSION";
    Exit 1
}

$projectName = "Stravaig.ConnOfficer";
$repoRoot = [System.IO.Path]::GetFullPath("$PSScriptRoot/..");
$projectPath = "$repoRoot/src/$projectName/$projectName.csproj";
$appIconSet = "$PSScriptRoot/MacOS/icon.iconset";
$infoPListPath = "$PSScriptRoot/MacOS/Info.plist";
$versionFile = "$repoRoot/version.txt";

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
$contentPListPath = "$contentsDir/Info.plist";

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
Copy-Item -Path $infoPListPath -Destination $contentPlistPath;

UpdateInfoPList -File $contentPListPath -Key "CFBundleVersion" -Type "string" -Value $stravaigAppVersion;
UpdateInfoPList -File $contentPListPath -Key "CFBundleShortVersionString" -Type "string" -Value $stravaigAppVersion;
