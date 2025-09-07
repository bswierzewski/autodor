using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

/// <summary>
/// Main entry point for registering the Products module with the dependency injection container.
/// Provides a unified registration method that configures both application and infrastructure layers.
/// </summary>
public static class ModuleRegistration
{
    /// <summary>
    /// Registers all Products module services with the dependency injection container.
    /// Configures both application layer (business logic) and infrastructure layer (data access, external services).
    /// </summary>
    /// <param name="services">The service collection to register dependencies in</param>
    /// <param name="configuration">Application configuration containing connection strings and external service settings</param>
    /// <param name="configure">Optional configurator action for enabling additional features like background synchronization</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddProducts(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        // Register application layer dependencies (business logic, APIs, handlers)
        services.AddApplication();
        
        // Register infrastructure layer dependencies (database, external services, background services)
        // Pass through the configurator to allow optional feature activation
        services.AddInfrastructure(configuration, configure);

        return services;
    }
}