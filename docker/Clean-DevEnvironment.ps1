[CmdletBinding()]
param(
    [Parameter()]
    [switch]$CleanMongoData,

    [Parameter()]
    [switch]$CleanRabbitMQData
)

# Import shared functions
. (Join-Path $PSScriptRoot 'SharedFunctions.ps1')

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Data directories
$mongoDataDir = Join-Path $PSScriptRoot 'mongodb' 'data'
$rabbitMQDataDir = Join-Path $PSScriptRoot 'rabbitmq' 'data'

# Main script
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  TSA Submissions Coding - Clean Development Environment   " -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

$runningServices = docker compose ps --services --filter "status=running" 2>$null

if($runningServices -and $runningServices.Count -gt 0) {
    Write-Warning "The following Docker services are currently running:"
    $runningServices | ForEach-Object { Write-Host "  - $_" }
    Write-Host ""
    $stop = Read-Host "Do you want to stop these services now? (y/N)"
    if ($stop -eq 'y' -or $stop -eq 'Y') {
        Write-Info "Stopping running Docker services..."
        docker compose down
        Write-Success "Docker services stopped."
    } else {
        Write-Error "Please stop the running Docker services before cleaning the development environment."
        exit 1
    }
}

if($CleanMongoData) {
    if (Test-Path $mongoDataDir) {
        Write-Info "Removing MongoDB data directory: $mongoDataDir"
        Remove-Item -Recurse -Force -Path $mongoDataDir
        Write-Success "MongoDB data directory removed."
    } else {
        Write-Warning "MongoDB data directory does not exist: $mongoDataDir"
    }
} else {
    Write-Info "Skipping MongoDB data directory cleanup."
}

if($CleanRabbitMQData) {
    if (Test-Path $rabbitMQDataDir) {
        Write-Info "Removing RabbitMQ data directory: $rabbitMQDataDir"
        Remove-Item -Recurse -Force -Path $rabbitMQDataDir
        Write-Success "RabbitMQ data directory removed."
    } else {
        Write-Warning "RabbitMQ data directory does not exist: $rabbitMQDataDir"
    }
} else {
    Write-Info "Skipping RabbitMQ data directory cleanup."
}

Write-Host ""
Write-Success "Development environment cleanup completed."
