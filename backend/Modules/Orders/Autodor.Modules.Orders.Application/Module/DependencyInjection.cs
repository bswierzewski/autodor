using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Application.Module;

/// <summary>
/// Provides dependency injection configuration for the Orders Application layer.
/// This class registers all application services, handlers, and MediatR components
/// required for the Orders module functionality.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all application layer services for the Orders module.
    /// Configures MediatR for command and event handling within the module.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to</param>
    /// <returns>The service collection with registered Orders application services</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR services for command/query/event handling
        // This enables the CQRS pattern and decoupled communication within the module
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}