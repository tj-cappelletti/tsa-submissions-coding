function ConvertFrom-SecureStringToPlainText {
    param([SecureString]$SecureString)
    return [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureString))
}

function Get-RabbitMQPasswordHash {
    param([securestring]$Password)

    Write-Info "Generating RabbitMQ password hash using Docker..."

    # Check if Docker is available
    $dockerCmd = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $dockerCmd) {
        Write-Error "Docker is not installed or not in PATH. Please install Docker to continue."
        exit 1
    }

    try {
        # Convert SecureString to plain text
        $passwordPlain = ConvertFrom-SecureStringToPlainText -SecureString $Password

        # Use RabbitMQ Docker image to hash the password
        $lines = docker run --rm rabbitmq:4-management rabbitmqctl hash_password $passwordPlain 2>&1

        $hash = $lines | Select-String -Pattern '^\S+$' | ForEach-Object { $_.Line }
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to generate password hash: $hash"
            exit 1
        }

        # The output is the base64-encoded hash
        return $hash.Trim()
    }
    catch {
        Write-Error "Failed to generate RabbitMQ password hash: $_"
        exit 1
    }
}

function Get-SecurePassword {
    param(
        [string]$Prompt
    )

    $password = Read-Host -Prompt $Prompt -AsSecureString

    $confirmPassword = Read-Host -Prompt "Confirm $Prompt" -AsSecureString

    $passwordPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

    $confirmPasswordPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($confirmPassword))

    if ($passwordPlain -ne $confirmPasswordPlain) {
        Write-Error "Passwords do not match. Please try again."
        return Get-SecurePassword -Prompt $Prompt
    }

    if ($passwordPlain.Length -lt 8) {
        Write-Warning "Password should be at least 8 characters for security."
        $continue = Read-Host "Continue anyway? (y/N)"
        if ($continue -ne 'y' -and $continue -ne 'Y') {
            return Get-SecurePassword -Prompt $Prompt
        }
    }

    return $password
}

function New-RandomString {
    param([int]$Length = 32)
    $bytes = New-Object byte[] $Length
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    return [Convert]::ToBase64String($bytes) -replace '[^a-zA-Z0-9]', ''
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Cyan
}

function Write-Error {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}
