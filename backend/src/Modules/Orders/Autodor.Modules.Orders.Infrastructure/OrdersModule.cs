using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Authorization;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Modules;
using Shared.Infrastructure.Persistence.Migrations;
using Autodor.Modules.Orders.Application;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Application.Options;
using Autodor.Modules.Orders.Domain;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Endpoints;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Autodor.Modules.Orders.Infrastructure.Services;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Shared.Contracts.Orders;
using Autodor.Modules.Orders.Application.API;

namespace Autodor.Modules.Orders.Infrastructure;

/// <summary>
/// Orders module - provides order management and PDF generation.
///
/// Features:
/// - Create, read, update, delete orders
/// - Order data management
/// - PDF document generation
///
/// Integration:
/// 1. Module is auto-discovered and loaded in AddModules()
/// 2. Endpoints are mapped in Program.cs via extension methods
/// 3. Database migrations run automatically on initialization
/// </summary>
public class OrdersModule : IModule
{
    /// <summary>
    /// Gets the unique name of the Orders module
    /// </summary>
    public string Name => ModuleConstants.ModuleName;

    /// <summary>
    /// Register Orders module services, DbContext, and command/query handlers
    /// </summary>
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<OrdersDatabaseOptions>(config);
                svc.ConfigureOptions<PolcarSalesOptions>(config);
            })
            .AddPostgres<OrdersDbContext, IOrdersDbContext>(
                provider => provider.GetRequiredService<IOptions<OrdersDatabaseOptions>>().Value.ConnectionString)
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        services.AddScoped(provider => new DistributorsSalesServiceSoapClient(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap));
        services.AddScoped<IOrdersRepository, PolcarOrderRepository>();
        services.AddScoped<IPdfDocumentService, PdfDocumentService>();

        // Register external API services
        services.AddScoped<IOrdersAPI, OrdersAPI>();
    }

    /// <summary>
    /// Configure middleware pipeline and map endpoints
    /// </summary>
    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Orders module endpoints
        endpoints.MapOrdersEndpoints();
    }

    /// <summary>
    /// Initializes the Orders module by running migrations
    /// </summary>
    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<OrdersDbContext>(serviceProvider).MigrateAsync(cancellationToken);
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
