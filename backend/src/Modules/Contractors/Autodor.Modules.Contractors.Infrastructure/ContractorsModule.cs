using Autodor.Modules.Contractors.Application;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using Autodor.Modules.Contractors.Application.Options;
using Autodor.Modules.Contractors.Domain;
using Autodor.Modules.Contractors.Infrastructure.Endpoints;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Shared.Contracts.Contractors;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Contractors.Infrastructure;

public class ContractorsModule : IModule
{
    public string Name => Module.Name;

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

    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Contractors module endpoints
        endpoints.MapContractorsEndpoints();
    }

    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<ContractorsDbContext>(serviceProvider).MigrateAsync(cancellationToken);
    }

}
