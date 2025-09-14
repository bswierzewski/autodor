using Autodor.Modules.Invoicing.Application.Abstractions;
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
        // Read configuration to determine which service to use
        var invoiceProvider = _configuration.GetValue<string>("InvoiceProvider") ?? "inFakt";

        return invoiceProvider.ToLower() switch
        {
            "infakt" => _serviceProvider.GetRequiredService<InFaktInvoiceService>(),
            "ifirma" => _serviceProvider.GetRequiredService<IFirmaInvoiceService>(),
            _ => _serviceProvider.GetRequiredService<IFirmaInvoiceService>() // Default to inFakt
        };
    }
}