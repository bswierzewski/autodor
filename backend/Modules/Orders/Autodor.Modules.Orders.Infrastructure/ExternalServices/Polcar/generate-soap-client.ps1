# Generate SOAP client for Polcar Sales Service
# Run this script to regenerate the SOAP client when the service contract changes

param(
    [string]$OutputPath = ".",
    [string]$Namespace = "Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated"
)

Write-Host "Generating SOAP client for Polcar Sales Service..." -ForegroundColor Green
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Namespace: $Namespace" -ForegroundColor Yellow

try {
    # Check if dotnet-svcutil is installed
    $svcutilCheck = dotnet tool list -g | Select-String "dotnet-svcutil"
    if (-not $svcutilCheck) {
        Write-Host "Installing dotnet-svcutil tool..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-svcutil
    }

    # Generate SOAP client
    dotnet-svcutil https://dedal.polcar.com/Sales/DistributorsSalesService.asmx?wsdl `
        --namespace "*,$Namespace" `
        --outputDir "$OutputPath\Generated" `
        --outputFile "$OutputPath\PolcarOrdersServiceClient.cs"

    Write-Host "SOAP client generated successfully!" -ForegroundColor Green
    Write-Host "File: $OutputPath\PolcarOrdersServiceClient.cs" -ForegroundColor Cyan
}
catch {
    Write-Host "Error generating SOAP client: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Done!" -ForegroundColor Green