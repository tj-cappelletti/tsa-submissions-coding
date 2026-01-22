[CmdletBinding()]
param(
    [Parameter()]
    [switch]$BuildDotNet,

    [Parameter()]
    [switch]$BuildJava
)

$dockerExists = Get-Command docker -ErrorAction SilentlyContinue

if (-not $dockerExists) {
    Write-Error "Docker is not installed or not available in the system PATH. Please install Docker to proceed."
    exit 1
}

$gitVersionExists = Get-Command gitversion -ErrorAction SilentlyContinue

$semVer = "local-build"
if ($gitVersionExists) {
    $semVer = gitversion /showvariable SemVer
    Write-Host "GitVersion detected. Using version tag: $semVer" -ForegroundColor Green
}
else {
    Write-Warning "GitVersion is not installed or not available in the system PATH. The version tag will default to 'local-build'."
}

# Supported Language Versions
$dotNetVersions = @("9.0")
$javaVersions = @("21")



if ($BuildDotNet) {
    foreach ($version in $dotNetVersions) {
        Write-Host ""
        Write-Host "Building Code Executor Runner Docker image for .NET $version..." -ForegroundColor Cyan
        
        docker build `
            --tag "code-executor-runner:$semVer-dotnet$version" `
            --file .\code-executor\Tsa.Submissions.Coding.CodeExecutor.Runner\Dockerfiles\DotNet.Dockerfile `
            --build-arg "LANG_VERSION=$version" `
            --quiet .

        Write-Host "Built Code Executor Runner Docker image for .NET $version." -ForegroundColor Green
    }
}

if ($BuildJava) {
    foreach ($version in $javaVersions) {
        Write-Host ""
        Write-Host "Building Code Executor Runner Docker image for Java $version..." -ForegroundColor Cyan

        docker build `
            --tag "code-executor-runner:$semVer-java$version" `
            --file .\code-executor\Tsa.Submissions.Coding.CodeExecutor.Runner\Dockerfiles\Java.Dockerfile `
            --build-arg "LANG_VERSION=$version" `
            --quiet .

        Write-Host "Built Code Executor Runner Docker image for Java $version." -ForegroundColor Green
    }
}
