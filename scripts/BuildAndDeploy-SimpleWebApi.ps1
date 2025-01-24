$WorkingDir = [System.IO.Path]::GetFullPath("$PSScriptRoot/../work");
$SrcDirectory = [System.IO.Path]::GetFullPath("$PSScriptRoot/../src/SimpleWebApi");
$containerRegistry = "conn-officer.stravaig.local:5050"
$k8sDir = "$WorkingDir/k8s";


function Update-BuildId($File){
    $BuildIdString = Get-Content $File;
    $BuildId = [int]$BuildIdString;
    $BuildId += 1;
    Set-Content -Path $File -Value $BuildId
    return $BuildId;
}

function Replace-InFiles($Path, $Token, $With){
    $Files = Get-ChildItem $Path;
    foreach($file in $Files) {
        $content = Get-Content -Path $file;
        $content = $content -replace "#{$Token}#", $With;
        Set-Content -Path $file -Value $content;
    }
}

function Apply-K8s($file){
    $fullPath = "$k8sDir/$file";
    if (-not (Test-Path -Path $fullPath)) {
        Write-Host "Path $fullPath does not exist."
        return
    }

    Write-Host "Applying $fullPath";
    Get-Content $fullPath | Write-Output
    kubectl apply -f "$fullPath"
}

if (-not (Test-Path -Path $WorkingDir)) {
    # Directory does not exist, so create it
    New-Item -ItemType Directory -Path $WorkingDir | Out-Null
}
else {
    Get-ChildItem -Path $WorkingDir -Recurse | Remove-Item -Recurse -Force
}

$BuildId = Update-BuildId -File "$SrcDirectory/BuildId.txt";
Write-Host "Build id is $BuildId"


# #Build and Push the docker container
docker build -t $containerRegistry/simple-web-api:latest -t $containerRegistry/simple-web-api:$BuildId $SrcDirectory
docker push $containerRegistry/simple-web-api:$BuildId

 New-Item -ItemType Directory -Path $k8sDir | Out-Null;
 Copy-Item -Path "$SrcDirectory/k8s/*" -Destination $k8sDir -Recurse -Force;


 $branchName = git branch --show-current;
 $commitSha = git rev-parse HEAD;
 Replace-InFiles -Path $k8sDir -Token BUILD_ID -With $BuildId
 Replace-InFiles -Path $k8sDir -Token BUILD_DATE -With (Get-Date -AsUTC).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
 Replace-InFiles -Path $k8sDir -Token CODE_BRANCH_NAME -With $branchName
 Replace-InFiles -Path $k8sDir -Token CODE_COMMIT_SHA -With $commitSha
 Replace-InFiles -Path $k8sDir -Token CONTAINER_REGISTRY -With $containerRegistry

Apply-K8s namespace.yml
Apply-K8s deploy.yml
