using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing;

public static class InvoicingModule
{
    public static readonly string Name = "Invoicing";

    public static IServiceCollection AddInvoicingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Name)
            .AddOptions(cfg =>
            {
                cfg.ConfigureOptions<InvoicingOptions>();
                cfg.ConfigureOptions<InFaktOptions>();
                cfg.ConfigureOptions<IFirmaOptions>();
            })
            .Build();

        // Register both HTTP clients (always available for testing and flexibility)
        services.AddInFaktHttpClient();
        services.AddIFirmaHttpClient();

        // Register both invoice service implementations
        services.AddScoped<InFaktInvoiceService>();
        services.AddScoped<IFirmaInvoiceService>();

        // Register IInvoiceService using factory pattern to select provider at runtime
        services.AddScoped<IInvoiceService>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<InvoicingOptions>>().Value;

            return options.Provider switch
            {
                InvoiceProvider.InFakt => serviceProvider.GetRequiredService<InFaktInvoiceService>(),
                InvoiceProvider.IFirma => serviceProvider.GetRequiredService<IFirmaInvoiceService>(),
                _ => throw new InvalidOperationException($"Unknown invoice provider: {options.Provider}")
            };
        });

        return services;
    }
}
