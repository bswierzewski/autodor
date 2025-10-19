using System.Reflection;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;
using Autodor.Modules.Products.Infrastructure.Persistence;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

/// <summary>
/// Provides dependency injection configuration for the Products infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services including database context, external services, and audit interceptors.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">Application configuration for connection strings and settings</param>
    /// <param name="configure">Optional configurator for module-specific settings</param>
    /// <returns>The configured service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services
            .AddMigrationService<ProductsDbContext>()
            .AddAuditableEntityInterceptor()
            .AddDomainEventDispatchInterceptor();
            
        services.AddDbContext<ProductsDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ProductsConnection"))
                   .AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
        });

        services.AddScoped<IProductsWriteDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());
        services.AddScoped<IProductsReadDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());

        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));
        services.AddScoped(provider => new ExternalServices.Polcar.Generated.ProductsSoapClient(ExternalServices.Polcar.Generated.ProductsSoapClient.EndpointConfiguration.ProductsSoap));
        services.AddScoped<IPolcarProductService, PolcarProductService>();

        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, Module>();

        return services;
    }
}