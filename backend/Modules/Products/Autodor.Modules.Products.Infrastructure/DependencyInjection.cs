using System.Reflection;
using Autodor.Modules.Products.Application;
using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Infrastructure.Abstractions;
using Autodor.Modules.Products.Infrastructure.Configuration;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.Options;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Autodor.Modules.Products.Infrastructure.Repositories;
using Autodor.Modules.Products.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

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
        // Rejestracja MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Rejestracja DbContext
        services.AddDbContext<ProductsDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ProductsConnection"));

            // This adds AuditableEntityInterceptor and DispatchDomainEventsInterceptor
            options.AddInterceptors(serviceProvider);
        });

        // Rejestracja opcji konfiguracyjnych
        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));

        // Register SOAP client
        services.AddScoped(provider => new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap));

        // Rejestracja serwisów
        services.AddScoped<IPolcarProductService, PolcarProductService>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Rejestracja serwisu do uruchamiania migracji
        services.AddHostedService<ProductsMigrationService>();

        // Konfiguracja opcjonalnych serwisów
        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        return services;
    }
}