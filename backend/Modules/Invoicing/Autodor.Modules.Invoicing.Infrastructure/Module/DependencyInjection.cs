using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Infrastructure.Services;

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

        // Register invoice services
        services.AddScoped<InFaktInvoiceService>();
        services.AddScoped<IFirmaInvoiceService>();
        services.AddScoped<IInvoiceServiceFactory, InvoiceServiceFactory>();

        return services;
    }
}