# Generated SOAP Client

This folder contains auto-generated SOAP client code for Polcar Products Service.

## Regenerating the client

To regenerate the SOAP client when the service contract changes, run:

```powershell
.\generate-soap-client.ps1
```

## Files

- `generate-soap-client.ps1` - PowerShell script to regenerate the client
- `Generated\PolcarProductsServiceClient.cs` - Generated SOAP client (created after running the script)

## Important Notes

- **DO NOT** manually edit the generated `PolcarProductsServiceClient.cs` file
- All manual changes will be lost when regenerating
- The generated code uses namespace: `Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated`
- If you need to customize behavior, create wrapper classes in the parent folder
