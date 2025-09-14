using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

/// <summary>
/// Provides the main entry point for registering all Products module services.
/// </summary>
public static class ModuleRegistration
{
    /// <summary>
    /// Registers all Products module services including application and infrastructure layers.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">Application configuration for connection strings and settings</param>
    /// <param name="configure">Optional configurator for module-specific features like synchronization</param>
    /// <returns>The configured service collection for method chaining</returns>
    public static IServiceCollection AddProducts(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        services.AddApplication();
        
        services.AddInfrastructure(configuration, configure);

        return services;
    }
}