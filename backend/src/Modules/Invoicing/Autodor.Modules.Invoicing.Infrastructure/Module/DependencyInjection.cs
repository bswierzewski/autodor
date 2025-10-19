using System.Reflection;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure.Module;

/// <summary>
/// Configures dependency injection for the Invoicing infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services including MediatR handlers and entity interceptors.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The configured service collection for method chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services
            .AddAuditableEntityInterceptor()
            .AddDomainEventDispatchInterceptor();

        // Configure options
        services.AddOptions<InFaktOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection("InvoicingModule:InFakt").Bind(options);
            });

        // Register HttpClient for InFakt
        services.AddHttpClient<Services.InFakt.InvoiceService>();
        services.AddHttpClient<Services.InFakt.ContractorService>();

        // Register invoice services
        services.AddScoped<IInvoiceServiceFactory, InvoiceServiceFactory>();

        // Register iFirma service
        services.AddScoped<Services.IFirma.InvoiceService>();

        // Register inFakt services
        services.AddScoped<Services.InFakt.InvoiceService>();
        services.AddScoped<Services.InFakt.ContractorService>();
        services.AddScoped<Services.InFakt.PreProcessor>();

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, InvoicingModule>();

        return services;
    }
}