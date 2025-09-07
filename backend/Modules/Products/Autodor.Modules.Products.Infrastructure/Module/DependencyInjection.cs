using System.Reflection;
using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.Module;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;
using Autodor.Modules.Products.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

public static class DependencyInjection
{
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

        // Konfiguracja opcjonalnych serwisów
        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        return services;
    }
}