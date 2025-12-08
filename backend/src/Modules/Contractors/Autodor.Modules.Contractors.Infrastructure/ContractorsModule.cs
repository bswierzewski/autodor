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
using Autodor.Modules.Contractors.Application;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Options;
using Autodor.Modules.Contractors.Domain;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Contractors.Infrastructure.Endpoints;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Modules.Contractors.Application.API;

namespace Autodor.Modules.Contractors.Infrastructure;

/// <summary>
/// Contractors module - provides contractor management.
///
/// Features:
/// - Create, read, update, delete contractors
/// - Contractor data management
///
/// Integration:
/// 1. Module is auto-discovered and loaded in AddModules()
/// 2. Endpoints are mapped in Program.cs via extension methods
/// 3. Database migrations run automatically on initialization
/// </summary>
public class ContractorsModule : IModule
{
    /// <summary>
    /// Gets the unique name of the Contractors module
    /// </summary>
    public string Name => ModuleConstants.ModuleName;

    /// <summary>
    /// Register Contractors module services, DbContext, and command/query handlers
    /// </summary>
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<ContractorsDatabaseOptions>(config);
            })
            .AddPostgres<ContractorsDbContext, IContractorsDbContext>(
                provider => provider.GetRequiredService<IOptions<ContractorsDatabaseOptions>>().Value.ConnectionString)
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        // Register external API services
        services.AddScoped<IContractorsAPI, ContractorsAPI>();
    }

    /// <summary>
    /// Configure middleware pipeline and map endpoints
    /// </summary>
    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Contractors module endpoints
        endpoints.MapContractorsEndpoints();
    }

    /// <summary>
    /// Initializes the Contractors module by running migrations
    /// </summary>
    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<ContractorsDbContext>(serviceProvider).MigrateAsync(cancellationToken);
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
