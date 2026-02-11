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

if ($runningServices -and $runningServices.Count -gt 0) {
    Write-Warning "The following Docker services are currently running:"
    $runningServices | ForEach-Object { Write-Host "  - $_" }
    Write-Host ""
    $stop = Read-Host "Do you want to stop these services now? (y/N)"
    if ($stop -eq 'y' -or $stop -eq 'Y') {
        Write-Info "Stopping running Docker services..."
        docker compose down
        Write-Success "Docker services stopped."
    }
    else {
        Write-Error "Please stop the running Docker services before cleaning the development environment."
        exit 1
    }
}

$dockerVolumes = docker volume ls --filter name=tsa-submissions-coding --quiet 2>$null

if ($CleanMongoData) {
    if ($dockerVolumes.Contains("tsa-submissions-coding_mongodb")) {
        Write-Info "Removing MongoDB Docker volume: tsa-submissions-coding_mongodb"
        try {
            docker volume rm tsa-submissions-coding_mongodb
            Write-Success "MongoDB Docker volume removed."
        } catch {
            Write-Warning "Failed to remove MongoDB Docker volume: tsa-submissions-coding_mongodb. Error: $_"
        }
    }
    else {
        Write-Warning "MongoDB Docker volume does not exist: tsa-submissions-coding_mongodb"
    }
}
else {
    Write-Info "Skipping MongoDB Docker volume cleanup."
}

if ($CleanRabbitMQData) {
    if ($dockerVolumes.Contains("tsa-submissions-coding_rabbitmq")) {
        Write-Info "Removing RabbitMQ Docker volume: tsa-submissions-coding_rabbitmq"
        try {
            docker volume rm tsa-submissions-coding_rabbitmq
            Write-Success "RabbitMQ Docker volume removed."
        } catch {
            Write-Warning "Failed to remove RabbitMQ Docker volume: tsa-submissions-coding_rabbitmq. Error: $_"
        }
    }
    else {
        Write-Warning "RabbitMQ Docker volume does not exist: tsa-submissions-coding_rabbitmq"
    }
}
else {
    Write-Info "Skipping RabbitMQ Docker volume cleanup."
}

Write-Host ""
Write-Success "Development environment cleanup completed."
