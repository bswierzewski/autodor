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
            "infakt" => _serviceProvider.GetRequiredService<InFaktInvoiceService>(),
            "ifirma" => _serviceProvider.GetRequiredService<IFirmaInvoiceService>(),
            _ => _serviceProvider.GetRequiredService<InFaktInvoiceService>() // Default to inFakt
        };
    }

    public IInvoicePreProcessor? GetInvoicePreProcessor()
    {
        var invoiceProvider = _configuration.GetValue<string>("InvoiceProvider") ?? "inFakt";

        return invoiceProvider.ToLower() switch
        {
            "infakt" => _serviceProvider.GetRequiredService<InFaktPreProcessor>(),
            "ifirma" => null, // IFirma doesn't need pre-processing
            _ => _serviceProvider.GetRequiredService<InFaktPreProcessor>() // Default to inFakt
        };
    }
}