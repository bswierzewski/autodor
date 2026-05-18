using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing;

public sealed class InvoicingModule : IModule
{
    public string Name => "Invoicing";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<InvoicingOptions>(configuration, InvoicingOptions.SectionName);
        services.AddValidatedOptions<InFaktOptions>(configuration, InFaktOptions.SectionName);
        services.AddValidatedOptions<IFirmaOptions>(configuration, IFirmaOptions.SectionName);

        // Register both HTTP clients (always available for testing and flexibility)
        services.AddInFaktHttpClient();
        services.AddIFirmaHttpClient();

        // Register both invoice service implementations as keyed services
        services.AddKeyedScoped<IInvoiceService, InFaktInvoiceService>(InvoiceProvider.InFakt);
        services.AddKeyedScoped<IInvoiceService, IFirmaInvoiceService>(InvoiceProvider.IFirma);
    }
}
