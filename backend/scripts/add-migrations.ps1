#!/usr/bin/env pwsh
param([ValidateSet('Contractors', 'Orders', 'Invoicing', 'All')][string]$Module = 'All')

# Navigate to backend directory (where solution is located)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Split-Path -Parent $scriptDir
Set-Location $backendDir

$modules = @(
    @{ Name = 'Contractors'; Context = 'ContractorsDbContext'; Project = 'src/Modules/Contractors/Autodor.Modules.Contractors' }
    @{ Name = 'Orders';      Context = 'OrdersDbContext';      Project = 'src/Modules/Orders/Autodor.Modules.Orders' }
    @{ Name = 'Invoicing';   Context = 'InvoicingDbContext';   Project = 'src/Modules/Invoicing/Autodor.Modules.Invoicing' }
)

# Filter modules if specific one is selected
if ($Module -ne 'All') {
    $modules = $modules | Where-Object { $_.Name -eq $Module }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Checking Migrations" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$created = 0
$skipped = 0

foreach ($m in $modules) {
    Write-Host "[$($m.Name)]" -ForegroundColor Cyan -NoNewline
    Write-Host " Checking changes..." -NoNewline

    # Check if project exists
    if (-not (Test-Path $m.Project)) {
        Write-Host " SKIPPED (project not found)" -ForegroundColor Yellow
        $skipped++
        continue
    }

    # Check for pending model changes
    $output = dotnet ef migrations has-pending-model-changes `
        --context $m.Context `
        --project $m.Project `
        --startup-project src/Host/Autodor.API `
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
    dotnet ef migrations add $timestamp `
        --context $m.Context `
        --project $m.Project `
        --startup-project src/Host/Autodor.API `
        --output-dir Infrastructure/Persistence/Migrations `
        | Out-Null

    if ($LASTEXITCODE -eq 0) {
        Write-Host " SUCCESS ($timestamp)" -ForegroundColor Green
        $created++
    } else {
        Write-Host " ERROR" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Created:  " -NoNewline
Write-Host $created -ForegroundColor Green
Write-Host "Skipped:  " -NoNewline
Write-Host $skipped -ForegroundColor Yellow
Write-Host ""
