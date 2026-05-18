#!/usr/bin/env pwsh

# Navigate to backend directory (where solution is located)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Join-Path (Split-Path -Parent $scriptDir) "backend"
Set-Location $backendDir

$contexts = @(
    @{ Context = 'ContractorsDbContext'; Project = 'modules/contractors/Autodor.Modules.Contractors' }
    @{ Context = 'OrdersDbContext';      Project = 'modules/orders/Autodor.Modules.Orders' }
)

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Checking Migrations" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$created = 0
$skipped = 0

foreach ($contextDefinition in $contexts) {
    Write-Host "[$($contextDefinition.Context)]" -ForegroundColor Cyan -NoNewline
    Write-Host " Checking changes..." -NoNewline

    # Check if project exists
    if (-not (Test-Path $contextDefinition.Project)) {
        Write-Host " SKIPPED (project not found)" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Check for pending model changes
    $output = dotnet ef migrations has-pending-model-changes `
        --context $contextDefinition.Context `
        --project $contextDefinition.Project `
        2>&1 | Out-String

    # Check if DbContext exists
    if ($output -match "No DbContext named") {
        Write-Host " SKIPPED (DbContext not found)" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Check for other errors (configuration issues, etc.)
    if ($output -match "Unable to create") {
        Write-Host " ERROR" -ForegroundColor Red
        Write-Host $output -ForegroundColor Red
        $skipped++
        continue
    }

    if ($output -match "No changes have been made") {
        Write-Host " no changes" -ForegroundColor Yellow
        $skipped++
        continue
    }

    Write-Host " CHANGES DETECTED!" -ForegroundColor Green
    Write-Host "  Creating migration..." -NoNewline

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $migrationOutput = dotnet ef migrations add $timestamp `
        --context $contextDefinition.Context `
        --project $contextDefinition.Project `
        --output-dir Infrastructure/Persistence/Migrations `
        2>&1 | Out-String

    if ($LASTEXITCODE -eq 0) {
        Write-Host " SUCCESS ($timestamp)" -ForegroundColor Green
        $created++
    } else {
        Write-Host " ERROR" -ForegroundColor Red
        Write-Host $migrationOutput -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Created:  " -NoNewline
Write-Host $created -ForegroundColor Green
Write-Host "Skipped:  " -NoNewline
Write-Host $skipped -ForegroundColor Yellow
Write-Host ""
