[CmdletBinding()]
param 
(
    [switch]$Reset = $false
)

function StopContainer($ContainerName){
    Write-Verbose "Stopping container '$ContainerName'..."
    $cmd = "docker stop $ContainerName"
    Write-Verbose $cmd
    Invoke-Expression $cmd
}

function DeleteContainer($ContainerName){
    $cmd = "docker rm $ContainerName"
    Write-Verbose $cmd
    Invoke-Expression $cmd
}

function RunContainer($ContainerName, $Tag, $PublishPort, $HostPort){
    $cmd = "docker run -d -p ${HostPort}:${PublishPort} --restart always --name $ContainerName ${ContainerName}:${Tag}"
    Write-Verbose $cmd
    Invoke-Expression $cmd
}

#Configure the local hosts file - This will require SuperUser Access on Linux and MacOS

function Setup-Hosts() {
    $hostsEntry = "127.0.0.1 conn-officer.stravaig.local"

    # Locate the hosts file based on the platform
    if ($IsWindows) {
        $hostsFilePath = "C:\Windows\System32\drivers\etc\hosts"
    } elseif ($IsLinux -or $IsMacOS) {
        $hostsFilePath = "/etc/hosts"
    } else {
        Write-Error "Unsupported operating system."
        Exit 1
    }

    # Check if the entry already exists
    $pattern = [regex]::Escape($hostsEntry);
    if (Get-Content -Path $hostsFilePath | Select-String -Pattern $pattern) {
        return;
    }

    # Add the entry to the hosts file
    try {
        # On Linux/macOS, you need to be a superuser to modify the hosts file
        if ($IsWindows) {
            # Append the entry in Windows
            Add-Content -Path $hostsFilePath -Value $hostsEntry
        } else {
            # Append entry for Linux/macOS using sudo
            Write-Host "Need to elevate privileges to add entry to the hosts file."
            "sudo bash -c `"echo '$hostsEntry' >> $hostsFilePath`"" | bash
        }
        Write-Output "Successfully added the entry to the hosts file."
    } catch {
        Write-Error "Failed to update the hosts file. Ensure you have administrative or root privileges."
        Exit 2
    }
}

Setup-Hosts;

# Set up a Docker Registry
# https://www.allisonthackston.com/articles/local-docker-registry.html

# Ports = HOST_PORT:CONTAINER_PORT

$registryContainerName = "registry"
$runningContainers = docker ps --format "{{.Names}}"
if ($runningContainers -contains $registryContainerName) {
    Write-Host "The $registryContainerName container is already running.";
    if ($Reset) {
        Write-Host "Resetting $registryContainerName";
        StopContainer($registryContainerName);
        DeleteContainer($registryContainerName);
        RunContainer -ContainerName $registryContainerName -Tag 2 -PublishPort 5000 -HostPort 5050;
    }
}
else{
    RunContainer -ContainerName $registryContainerName -Tag 2 -PublishPort 5000 -HostPort 5050;
}

