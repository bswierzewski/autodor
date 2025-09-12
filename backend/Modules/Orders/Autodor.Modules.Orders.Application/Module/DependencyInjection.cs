using System.Reflection;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Application.Module;

/// <summary>
/// Configures dependency injection for the Orders application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application layer services including validators, MediatR with behaviors, and request handlers.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The configured service collection for method chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidators();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddLoggingBehavior()
               .AddUnhandledExceptionBehavior()
               .AddValidationBehavior()
               .AddAuthorizationBehavior()
               .AddPerformanceMonitoringBehavior();
        });
        
        return services;
    }
}