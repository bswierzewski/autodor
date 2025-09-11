using System.Reflection;
using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.Module;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;
using Autodor.Modules.Products.Infrastructure.Persistence;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

/// <summary>
/// Dependency injection configuration for the Products module infrastructure layer.
/// Configures database context, external services, background services, and their dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure layer services for the Products module.
    /// Includes database context, external service clients, configuration options, and background services.
    /// </summary>
    /// <param name="services">The service collection to register dependencies in</param>
    /// <param name="configuration">Application configuration for connection strings and external service settings</param>
    /// <param name="configure">Optional configurator for enabling additional features like synchronization</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        // Register Entity Framework interceptors for auditing and domain event dispatching
        services.AddAuditableEntityInterceptor();
        services.AddDomainEventDispatchInterceptor();

        // Configure and register the Entity Framework database context
        // Uses PostgreSQL as the database provider with connection string from configuration
        services.AddDbContext<ProductsDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("ProductsConnection"));
        });

        // Register DbContext interfaces to support CQRS pattern separation
        // Both read and write operations use the same context but with different interfaces
        services.AddScoped<IProductsWriteDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());
        services.AddScoped<IProductsReadDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());

        // Register MediatR for command/query handling within the infrastructure layer
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register configuration options for external Polcar service integration
        // Binds settings from appsettings.json to strongly-typed options class
        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));

        // Register SOAP client for Polcar external service communication
        // Uses generated client with default endpoint configuration
        services.AddScoped(provider => new ExternalServices.Polcar.Generated.ProductsSoapClient(ExternalServices.Polcar.Generated.ProductsSoapClient.EndpointConfiguration.ProductsSoap));

        // Register business service implementations
        // Provides abstraction over external SOAP service with retry policies and error handling
        services.AddScoped<IPolcarProductService, PolcarProductService>();

        // Configure optional services using fluent configurator pattern
        // Allows consumers to selectively enable features like background synchronization
        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        return services;
    }
}