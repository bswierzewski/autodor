using Autodor.Modules.Contractors.Application.Module;
using Autodor.Modules.Contractors.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Infrastructure;

/// <summary>
/// Provides the main entry point for registering the complete Contractors module.
/// This static class orchestrates the registration of both Application and Infrastructure layers,
/// providing a single method for integrating the entire contractors domain into the host application.
/// Follows the modular monolith pattern by encapsulating all module dependencies behind a single interface.
/// </summary>
public static class ModuleRegistration
{
    /// <summary>
    /// Registers the complete Contractors module including all Application and Infrastructure services.
    /// This method provides a single integration point for the host application to register all
    /// contractors-related functionality, including CQRS handlers, database context, and domain services.
    /// Ensures proper dependency ordering and modular architecture compliance.
    /// </summary>
    /// <param name="services">The service collection to register the contractors module into</param>
    /// <param name="configuration">The application configuration containing database connections and settings</param>
    /// <returns>The service collection with the complete contractors module registered for method chaining</returns>
    public static IServiceCollection AddContractors(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Application layer services first
        // This includes MediatR handlers, command/query processors, and application services
        services.AddApplication();
        
        // Register Infrastructure layer services with configuration
        // This includes database context, Entity Framework setup, and external service integrations
        services.AddInfrastructure(configuration);

        // Return services for fluent configuration chaining
        // Allows the host application to continue adding other modules or services
        return services;
    }
}