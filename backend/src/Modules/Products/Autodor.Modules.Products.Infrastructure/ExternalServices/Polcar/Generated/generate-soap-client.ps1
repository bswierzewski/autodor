# Generate SOAP client for Polcar Products Service
# Run this script to regenerate the SOAP client when the service contract changes

param(
    [string]$OutputPath = ".",
    [string]$Namespace = "Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated"
)

# Disable telemetry
$env:DOTNET_SVCUTIL_TELEMETRY_OPTOUT = "1"

Write-Host "Generating SOAP client for Polcar Products Service..." -ForegroundColor Green
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Namespace: $Namespace" -ForegroundColor Yellow

try {
    # Check if dotnet-svcutil is installed
    $svcutilCheck = dotnet tool list -g | Select-String "dotnet-svcutil"
    if (-not $svcutilCheck) {
        Write-Host "Installing dotnet-svcutil tool..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-svcutil
    }

    # Remove existing file if it exists
    $outputFile = "$OutputPath\PolcarProductsServiceClient.cs"
    if (Test-Path $outputFile) {
        Write-Host "Removing existing file: $outputFile" -ForegroundColor Yellow
        Remove-Item $outputFile -Force
    }

    # Generate SOAP client
    dotnet-svcutil https://dedal.polcar.com/Dystrybutorzy/Products.asmx?wsdl `
        --namespace "*,$Namespace" `
        --outputDir "$OutputPath\" `
        --outputFile "$OutputPath\PolcarProductsServiceClient.cs"

    Write-Host "SOAP client generated successfully!" -ForegroundColor Green
    Write-Host "File: $OutputPath\PolcarProductsServiceClient.cs" -ForegroundColor Cyan
}
catch {
    Write-Host "Error generating SOAP client: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Done!" -ForegroundColor Green