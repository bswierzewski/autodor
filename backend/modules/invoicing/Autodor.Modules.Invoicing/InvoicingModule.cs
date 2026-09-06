using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Features.CreateInvoice;
using Autodor.Modules.Invoicing.Features.CreateInvoices;
using Autodor.Modules.Invoicing.Infrastructure.Email;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing;

public sealed class InvoicingModule : IModuleEndpoint
{
    public string Name => "Invoicing";

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        CreateInvoiceEndpoint.Map(endpoints);
    }

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var invoicingOptions = services.AddValidatedOptionsSnapshot<InvoicingOptions>(configuration, InvoicingOptions.SectionName);
        services.AddValidatedOptions<InFaktOptions>(configuration, InFaktOptions.SectionName);
        services.AddValidatedOptions<IFirmaOptions>(configuration, IFirmaOptions.SectionName);
        services.AddValidatedOptions<SmtpOptions>(configuration, SmtpOptions.SectionName);

        // Register both HTTP clients (always available for testing and flexibility)
        services.AddInFaktHttpClient();
        services.AddIFirmaHttpClient();

        switch (invoicingOptions.Provider)
        {
            case InvoiceProvider.InFakt:
                services.AddScoped<IInvoiceService, InFaktInvoiceService>();
                break;
            case InvoiceProvider.IFirma:
                services.AddScoped<IInvoiceService, IFirmaInvoiceService>();
                break;
            default:
                throw new InvalidOperationException($"Unsupported invoice provider: {invoicingOptions.Provider}.");
        }

        services.AddSingleton<IEmailSender, MailKitEmailSender>();
        services.AddHostedService<CreateInvoicesScheduler>();
    }
}
