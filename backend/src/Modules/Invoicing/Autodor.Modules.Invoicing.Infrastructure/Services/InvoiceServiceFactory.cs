using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class InvoiceServiceFactory : IInvoiceServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public InvoiceServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public IInvoiceService GetInvoiceService()
    {
        var invoiceProvider = _configuration.GetValue<string>("InvoiceProvider") ?? "inFakt";

        return invoiceProvider.ToLower() switch
        {
            "infakt" => _serviceProvider.GetRequiredService<InFakt.InvoiceService>(),
            "ifirma" => _serviceProvider.GetRequiredService<IFirma.InvoiceService>(),
            _ => _serviceProvider.GetRequiredService<InFakt.InvoiceService>() // Default to inFakt
        };
    }

    public IInvoicePreProcessor? GetInvoicePreProcessor()
    {
        var invoiceProvider = _configuration.GetValue<string>("InvoiceProvider") ?? "inFakt";

        return invoiceProvider.ToLower() switch
        {
            "infakt" => _serviceProvider.GetRequiredService<InFakt.PreProcessor>(),
            "ifirma" => null, // IFirma doesn't need pre-processing
            _ => _serviceProvider.GetRequiredService<InFakt.PreProcessor>() // Default to inFakt
        };
    }
}