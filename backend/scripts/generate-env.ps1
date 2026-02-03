# Generate .env.example file using BuildingBlocks.Tools.EnvGenerator
# This script builds and runs the EnvGenerator tool to create .env.example in the backend directory

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "  .env.example Generator" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

# Get the directory where this script is located (scripts folder)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
# Go up one level to backend directory
$backendDir = Split-Path -Parent $scriptDir
$toolProject = Join-Path $backendDir "src\BuildingBlocks\BuildingBlocks.Tools.EnvGenerator\BuildingBlocks.Tools.EnvGenerator.csproj"
$targetDirectory = $backendDir

Write-Host "Building EnvGenerator tool..." -ForegroundColor Yellow
dotnet build "$toolProject" --nologo --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Running EnvGenerator..." -ForegroundColor Yellow
Write-Host ""

dotnet run --project "$toolProject" --no-build -- "$targetDirectory"

Write-Host ""
Write-Host "===========================================" -ForegroundColor Green
Write-Host "  Done!" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Green
