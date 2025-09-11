using System.Reflection;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Application.Module;

/// <summary>
/// Provides dependency injection configuration for the Contractors Application layer.
/// This static class centralizes the registration of all application-layer services,
/// following the modular architecture pattern by encapsulating application-specific dependencies.
/// Ensures proper CQRS setup and service registration for the contractors domain.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all application layer services for the Contractors module into the dependency injection container.
    /// This method configures MediatR for CQRS pattern implementation, automatically discovering and registering
    /// all command handlers, query handlers, and related services within the application assembly.
    /// Essential for enabling the command/query processing pipeline in the contractors domain.
    /// </summary>
    /// <param name="services">The service collection to register dependencies into</param>
    /// <returns>The service collection with registered application services for method chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add FluentValidation validators from the current assembly
        services.AddValidators();

        // Register MediatR with automatic service discovery from the current assembly
        // This enables CQRS pattern by registering all IRequestHandler implementations
        // including command handlers, query handlers, and notification handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddLoggingBehavior();
            cfg.AddUnhandledExceptionBehavior();
            cfg.AddAuthorizationBehavior();
            cfg.AddValidationBehavior();
            cfg.AddPerformanceMonitoringBehavior();
        });

        // Return services for fluent configuration chaining
        // Allows additional configuration to be chained after application registration
        return services;
    }
}