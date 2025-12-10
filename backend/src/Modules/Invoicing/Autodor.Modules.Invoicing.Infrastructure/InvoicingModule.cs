using Autodor.Modules.Invoicing.Application;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain;
using Autodor.Modules.Invoicing.Infrastructure.Endpoints;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Authorization;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Modules;

namespace Autodor.Modules.Invoicing.Infrastructure;

/// <summary>
/// Invoicing module - provides invoicing and payment processing.
///
/// Features:
/// - Invoice management
/// - Payment processing
/// - External invoicing service integration
///
/// Integration:
/// 1. Module is auto-discovered and loaded in AddModules()
/// 2. Endpoints are mapped in Program.cs via extension methods
/// </summary>
public class InvoicingModule : IModule
{
    /// <summary>
    /// Gets the unique name of the Invoicing module
    /// </summary>
    public string Name => ModuleConstants.ModuleName;

    /// <summary>
    /// Register Invoicing module services and command/query handlers
    /// </summary>
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<InvoicingOptions>(config);
                svc.ConfigureOptions<InFaktOptions>(config);
                svc.ConfigureOptions<IFirmaOptions>(config);
            })
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        // Register HTTP clients for external invoicing services
        services.AddIFirmaHttpClient();
        services.AddInFaktHttpClient();

        // Register invoice service implementations with keyed services
        services.AddKeyedScoped<IInvoiceService, Services.InFakt.Services.InFaktInvoiceService>(InvoiceProvider.InFakt);
        services.AddKeyedScoped<IInvoiceService, Services.IFirma.Services.IFirmaInvoiceService>(InvoiceProvider.IFirma);
    }

    /// <summary>
    /// Configure middleware pipeline and map endpoints
    /// </summary>
    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Invoicing module endpoints
        endpoints.MapInvoicingEndpoints();
    }

    /// <summary>
    /// Define permissions available in this module
    /// </summary>
    public IEnumerable<Permission> GetPermissions()
    {
        return [];
    }

    /// <summary>
    /// Define roles available in this module
    /// </summary>
    public IEnumerable<Role> GetRoles()
    {
        return [];
    }
}
