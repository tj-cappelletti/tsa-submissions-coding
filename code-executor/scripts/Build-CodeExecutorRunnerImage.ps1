[CmdletBinding()]
param(
    [Parameter()]
    [switch]$BuildDotNet,

    [Parameter()]
    [switch]$BuildJava,

    [Parameter()]
    [ValidateSet("docker", "podman")]
    [string]$ContainerEngine = $null
)

function Get-ContainerEngine {
    param([string]$PreferredEngine)

    if ($PreferredEngine) {
        $cmd = Get-Command $PreferredEngine -ErrorAction SilentlyContinue
        if (-not $cmd) {
            throw "$PreferredEngine is not installed or not available in PATH."
        }
        return $PreferredEngine
    }

    if (Get-Command docker -ErrorAction SilentlyContinue) {
        Write-Information "Docker is available. Using Docker as the container engine." -InformationAction Continue
        return "docker"
    }

    if (Get-Command podman -ErrorAction SilentlyContinue) {
        Write-Information "Podman is available. Using Podman as the container engine." -InformationAction Continue
        return "podman"
    }

    throw "Neither Docker nor Podman is installed or available in PATH."
}

$containerEngine = Get-ContainerEngine -PreferredEngine $ContainerEngine

$gitVersionExists = Get-Command gitversion -ErrorAction SilentlyContinue
$semVer = "local-build"
if ($gitVersionExists) {
    $semVer = gitversion /showvariable SemVer
    Write-Host "GitVersion detected. Using version tag: $semVer" -ForegroundColor Green
}
else {
    Write-Warning "GitVersion is not installed or not available in the system PATH. The version tag will default to 'local-build'."
}

$dotNetVersions = @("9.0", "10.0")
$javaVersions = @("21")

if ($BuildDotNet) {
    foreach ($version in $dotNetVersions) {
        Write-Host ""
        Write-Host "Building Code Executor Runner image for .NET $version using $containerEngine..." -ForegroundColor Cyan

        & $containerEngine build `
            --tag "code-executor-runner:$semVer-dotnet$version" `
            --file .\code-executor\Tsa.Submissions.Coding.CodeExecutor.Runner\Dockerfiles\DotNet.Dockerfile `
            --build-arg "LANG_VERSION=$version" `
            --quiet .

        Write-Host "Built Code Executor Runner image for .NET $version." -ForegroundColor Green
    }
}

if ($BuildJava) {
    foreach ($version in $javaVersions) {
        Write-Host ""
        Write-Host "Building Code Executor Runner image for Java $version using $containerEngine..." -ForegroundColor Cyan

        & $containerEngine build `
            --tag "code-executor-runner:$semVer-java$version" `
            --file .\code-executor\Tsa.Submissions.Coding.CodeExecutor.Runner\Dockerfiles\Java.Dockerfile `
            --build-arg "LANG_VERSION=$version" `
            --quiet .

        Write-Host "Built Code Executor Runner image for Java $version." -ForegroundColor Green
    }
}
