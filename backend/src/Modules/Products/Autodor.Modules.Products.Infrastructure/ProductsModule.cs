using Autodor.Modules.Products.Application;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Application.API;
using Autodor.Modules.Products.Application.Options;
using Autodor.Modules.Products.Domain;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Autodor.Shared.Contracts.Products;
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

namespace Autodor.Modules.Products.Infrastructure;

/// <summary>
/// Products module - provides product management with synchronization support.
///
/// Features:
/// - Create, read, update, delete products
/// - Product data management
/// - Product synchronization
///
/// Integration:
/// 1. Module is auto-discovered and loaded in AddModules()
/// 2. Endpoints are mapped in Program.cs via extension methods
/// 3. Database migrations run automatically on initialization
/// </summary>
public class ProductsModule : IModule
{
    /// <summary>
    /// Gets the unique name of the Products module
    /// </summary>
    public string Name => ModuleConstants.ModuleName;

    /// <summary>
    /// Register Products module services, DbContext, and command/query handlers
    /// </summary>
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<ProductsDatabaseOptions>(config);
                svc.ConfigureOptions<PolcarProductsOptions>(config);
            })
            .AddPostgres<ProductsDbContext, IProductsDbContext>(
                provider => provider.GetRequiredService<IOptions<ProductsDatabaseOptions>>().Value.ConnectionString)
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        services.AddScoped(provider => new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap));
        services.AddScoped<IPolcarProductService, PolcarProductService>();
        services.AddScoped<IProductsAPI, ProductsAPI>();

        services.AddHostedService<ProductsSynchronizationService>();
    }

    /// <summary>
    /// Configure middleware pipeline and map endpoints
    /// </summary>
    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Products module endpoints
        // endpoints.MapProductsEndpoints();
    }

    /// <summary>
    /// Initializes the Products module by running migrations
    /// </summary>
    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<ProductsDbContext>(serviceProvider).MigrateAsync(cancellationToken);
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
