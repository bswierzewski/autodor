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
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

public class ProductsModule : IModule
{
    public string Name => Module.Name;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<PolcarProductsOptions>(config);
            })
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        services.AddSingleton<IProductsRepository, InMemoryProductsRepository>();
        services.AddScoped(provider => new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap));
        services.AddScoped<IPolcarProductService, PolcarProductService>();
        services.AddScoped<IProductsAPI, ProductsAPI>();

        services.AddHostedService<ProductsSynchronizationService>();
    }

    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
    }

    public Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
