using Autodor.Modules.Orders.Application.Module;
using Autodor.Modules.Orders.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Infrastructure;

/// <summary>
/// Provides the main registration entry point for the Orders module.
/// This class orchestrates the registration of all Orders module components
/// including application and infrastructure layers in the correct order.
/// </summary>
public static class ModuleRegistration
{
    /// <summary>
    /// Registers all services for the Orders module including application and infrastructure layers.
    /// This method provides a single entry point for configuring the complete Orders module.
    /// </summary>
    /// <param name="services">The service collection to add Orders module services to</param>
    /// <param name="configuration">Application configuration for module-specific settings</param>
    /// <returns>The service collection with registered Orders module services</returns>
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration)
    {
        // Register application layer services first (commands, queries, handlers, validators)
        // This establishes the business logic and application service contracts
        services.AddApplication();
        
        // Register infrastructure layer services (database, external services, repositories)
        // This provides concrete implementations for application layer abstractions
        services.AddInfrastructure(configuration);

        return services;
    }
}