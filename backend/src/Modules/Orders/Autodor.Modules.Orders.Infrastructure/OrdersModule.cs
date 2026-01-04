using Autodor.Modules.Orders.Application;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Application.API;
using Autodor.Modules.Orders.Application.Options;
using Autodor.Modules.Orders.Domain;
using Autodor.Modules.Orders.Infrastructure.Endpoints;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Autodor.Modules.Orders.Infrastructure.Services;
using Autodor.Shared.Contracts.Orders;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure;

public class OrdersModule : IModule
{
    public string Name => Module.Name;

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

    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Orders module endpoints
        endpoints.MapOrdersEndpoints();
    }

    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<OrdersDbContext>(serviceProvider).MigrateAsync(cancellationToken);
    }

}
