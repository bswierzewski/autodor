using System.Reflection;
using Autodor.Modules.Products.Application;
using Autodor.Modules.Products.Application.Interfaces;
using Autodor.Modules.Products.Infrastructure.Abstractions;
using Autodor.Modules.Products.Infrastructure.Api;
using Autodor.Modules.Products.Infrastructure.Configuration;
using Autodor.Modules.Products.Infrastructure.Options;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Autodor.Modules.Products.Infrastructure.Services;
using Autodor.Shared.Contracts.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure;

namespace Autodor.Modules.Products.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProducts(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration, configure);

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        // ProductsDbContext
        services.AddModule<ProductsDbContext>(
            configureDbContext: (serviceProvider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ProductsConnection"));
            }
        );

        // Rejestracja interfejsów DbContext
        services.AddScoped<IProductsWriteDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());
        services.AddScoped<IProductsReadDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());

        // Rejestracja MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Rejestracja opcji konfiguracyjnych
        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));

        // Register SOAP client
        services.AddScoped(provider => new ExternalServices.Polcar.Generated.ProductsSoapClient(ExternalServices.Polcar.Generated.ProductsSoapClient.EndpointConfiguration.ProductsSoap));

        // Rejestracja serwisów
        services.AddScoped<IPolcarProductService, PolcarProductService>();

        // Rejestracja publicznego API dla innych modułów
        services.AddScoped<IProductsApi, ProductsApi>();

        // Konfiguracja opcjonalnych serwisów
        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        return services;
    }
}