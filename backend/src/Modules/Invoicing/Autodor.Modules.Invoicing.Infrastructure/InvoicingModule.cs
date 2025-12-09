using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Authorization;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Modules;
using Autodor.Modules.Invoicing.Application;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain;
using Autodor.Modules.Invoicing.Infrastructure.Endpoints;
using Autodor.Modules.Invoicing.Infrastructure.Services;

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
                svc.ConfigureOptions<InFaktOptions>(config);
                svc.ConfigureOptions<IFirmaOptions>(config);
            })
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        // Register HttpClient for InFakt
        services.AddHttpClient<Services.InFakt.InFaktClient>();

        // Register invoice services
        services.AddScoped<IInvoiceServiceFactory, InvoiceServiceFactory>();

        // Register iFirma services
        services.AddScoped<Services.IFirma.IFirmaClient>();
        services.AddScoped<Services.IFirma.InvoiceService>();

        // Register inFakt services
        services.AddScoped<Services.InFakt.InFaktClient>();
        services.AddScoped<Services.InFakt.ContractorService>();
        services.AddScoped<Services.InFakt.InvoiceService>();
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
