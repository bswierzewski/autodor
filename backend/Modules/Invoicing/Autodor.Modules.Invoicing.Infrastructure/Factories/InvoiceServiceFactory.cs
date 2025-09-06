using Autodor.Modules.Invoicing.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Factories;

public class InvoiceServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InvoiceServiceFactory> _logger;

    public InvoiceServiceFactory(
        IServiceProvider serviceProvider, 
        IConfiguration configuration,
        ILogger<InvoiceServiceFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public IInvoiceService CreateInvoiceService()
    {
        var providerType = _configuration["InvoicingModule:InvoiceProvider"] ?? "Mock";
        
        _logger.LogInformation("Creating invoice service with provider: {ProviderType}", providerType);

        return providerType.ToLowerInvariant() switch
        {
            "infakt" => CreateInFaktService(),
            "mock" => _serviceProvider.GetRequiredService<Services.MockInvoiceService>(),
            _ => throw new NotSupportedException($"Invoice provider '{providerType}' is not supported")
        };
    }

    public IPdfGeneratorService CreatePdfGeneratorService()
    {
        var providerType = _configuration["InvoicingModule:PdfProvider"] ?? "Mock";
        
        _logger.LogInformation("Creating PDF generator service with provider: {ProviderType}", providerType);

        return providerType.ToLowerInvariant() switch
        {
            "itext" => CreateITextPdfService(),
            "dinktopdf" => CreateDinkToPdfService(),
            "mock" => _serviceProvider.GetRequiredService<Services.MockPdfGeneratorService>(),
            _ => throw new NotSupportedException($"PDF provider '{providerType}' is not supported")
        };
    }

    private IInvoiceService CreateInFaktService()
    {
        // W rzeczywistej implementacji tutaj byłaby konfiguracja InFakt API
        _logger.LogInformation("Creating InFakt invoice service (not implemented - falling back to mock)");
        return _serviceProvider.GetRequiredService<Services.MockInvoiceService>();
    }

    private IPdfGeneratorService CreateITextPdfService()
    {
        // W rzeczywistej implementacji tutaj byłaby konfiguracja iTextSharp
        _logger.LogInformation("Creating iText PDF generator service (not implemented - falling back to mock)");
        return _serviceProvider.GetRequiredService<Services.MockPdfGeneratorService>();
    }

    private IPdfGeneratorService CreateDinkToPdfService()
    {
        // W rzeczywistej implementacji tutaj byłaby konfiguracja DinkToPdf
        _logger.LogInformation("Creating DinkToPdf generator service (not implemented - falling back to mock)");
        return _serviceProvider.GetRequiredService<Services.MockPdfGeneratorService>();
    }
}