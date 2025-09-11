using System.Reflection;
using Autodor.Modules.Products.Application.API;
using Autodor.Shared.Contracts.Products;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Application.Module;

/// <summary>
/// Dependency injection configuration for the Products module application layer.
/// Registers business logic services, APIs, and command/query handlers.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all application layer services for the Products module.
    /// Includes MediatR handlers, domain services, and inter-module communication APIs.
    /// </summary>
    /// <param name="services">The service collection to register dependencies in</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidators();

        // Register MediatR for command/query pattern implementation
        // Scans current assembly for handlers, behaviors, and processors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddLoggingBehavior();
            cfg.AddUnhandledExceptionBehavior();
            cfg.AddAuthorizationBehavior();
            cfg.AddValidationBehavior();
            cfg.AddPerformanceMonitoringBehavior();
        });
        
        // Register Products API for inter-module communication
        // Allows other modules to query product information through defined contracts
        services.AddScoped<IProductsAPI, ProductsAPI>();
        
        return services;
    }
}