#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Sets up development environment configuration for TSA Submissions Coding platform.

.DESCRIPTION
    This script generates the .env file and RabbitMQ definitions.json file with secure passwords.
    It prompts for MongoDB and RabbitMQ passwords, generates a random Erlang cookie,
    and uses Docker to hash the RabbitMQ password for the definitions file.

.PARAMETER MongoPassword
    MongoDB root password. If not provided, will prompt securely.

.PARAMETER RabbitMQPassword
    RabbitMQ password. If not provided, will prompt securely.

.PARAMETER Force
    Overwrite existing files without prompting.

.EXAMPLE
    .\Setup-DevEnvironment.ps1
    Prompts for all passwords and generates configuration files.

.EXAMPLE
    .\Setup-DevEnvironment.ps1 -Force
    Regenerates configuration files without confirmation prompts.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [SecureString]$MongoPassword,

    [Parameter()]
    [SecureString]$RabbitMQPassword,

    [Parameter()]
    [switch]$Force
)

# Import shared functions
. (Join-Path $PSScriptRoot 'SharedFunctions.ps1')

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$envFile = Join-Path $scriptDir '.env'
$definitionsFile = Join-Path $scriptDir 'rabbitmq' 'definitions.json'
$definitionsTemplateFile = Join-Path $scriptDir 'rabbitmq' 'definitions.template.json'

# Main script
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  TSA Submissions Coding - Development Environment Setup   " -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Check for existing files
$filesExist = (Test-Path $envFile) -or (Test-Path $definitionsFile)
if ($filesExist -and -not $Force) {
    Write-Warning "Configuration files already exist:"

    if (Test-Path $envFile) { Write-Host "  - $envFile" }

    if (Test-Path $definitionsFile) { Write-Host "  - $definitionsFile" }

    Write-Host ""

    $overwrite = Read-Host "Overwrite existing files? (y/N)"

    if ($overwrite -ne 'y' -and $overwrite -ne 'Y') {
        Write-Info "Setup cancelled."
        exit 0
    }
}

# Collect passwords
Write-Info "Please provide passwords for the development environment."
Write-Host ""

$mongoPasswordSecure = Get-SecurePassword -Prompt "MongoDB root password" -ExistingPassword $MongoPassword
$mongoPasswordPlain = ConvertFrom-SecureStringToPlainText -SecureString $mongoPasswordSecure

$rabbitMQPasswordSecure = Get-SecurePassword -Prompt "RabbitMQ password" -ExistingPassword $RabbitMQPassword
$rabbitMQPasswordPlain = ConvertFrom-SecureStringToPlainText -SecureString $rabbitMQPasswordSecure

Write-Host ""
Write-Info "Generating random Erlang cookie..."
$erlangCookie = New-RandomString -Length 32

Write-Host ""
$rabbitMQPasswordHash = Get-RabbitMQPasswordHash -Password $rabbitMQPasswordSecure

# Generate .env file
Write-Host ""
Write-Info "Writing .env file..."
$envFileContent = @"
COMPOSE_PROJECT_NAME=tsa-submissions-coding
MONGO_ROOT_PASSWORD=$mongoPasswordPlain
RABBITMQ_DEFAULT_PASS=$rabbitMQPasswordPlain
RABBITMQ_ERLANG_COOKIE=$erlangCookie
"@

$envFileContent | Out-File -FilePath $envFile -Encoding utf8 -NoNewline
Write-Success "Created $envFile"

# Generate definitions.json file
Write-Info "Writing definitions.json file..."
$rabbitMQDefinitions = Get-Content -Path $definitionsTemplateFile -Raw | ConvertFrom-Json -Depth 10
$rabbitMQDefinitions.users[0].password_hash = $rabbitMQPasswordHash

$definitionsContent = $rabbitMQDefinitions | ConvertTo-Json -Depth 10
$definitionsContent | Out-File -FilePath $definitionsFile -Encoding utf8 -NoNewline
Write-Success "Created $definitionsFile"

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "  ✅ Setup Complete!                                        " -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Info "Configuration files have been generated:"
Write-Host "  ✓ .env" -ForegroundColor Green
Write-Host "  ✓ rabbitmq/definitions.json" -ForegroundColor Green
Write-Host ""
Write-Info "Credentials summary:"
Write-Host "  • MongoDB User: root" -ForegroundColor White
Write-Host "  • MongoDB Password: $mongoPasswordPlain" -ForegroundColor White
Write-Host "  • RabbitMQ User: coding" -ForegroundColor White
Write-Host "  • RabbitMQ Password: $rabbitMQPasswordPlain" -ForegroundColor White
Write-Host ""
Write-Warning "Store these credentials securely!"
Write-Host ""
Write-Info "Next steps:"
Write-Host "  1. Review the generated .env file" -ForegroundColor White
Write-Host "  2. Start the environment: docker compose up -d" -ForegroundColor White
Write-Host "  3. Access RabbitMQ Management: http://localhost:15672" -ForegroundColor White
Write-Host "  4. Access Mongo Express: http://localhost:8081" -ForegroundColor White
Write-Host ""
