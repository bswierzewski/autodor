using System.Reflection;
using Autodor.Modules.Products.Application.API;
using Autodor.Shared.Contracts.Products;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Application.Module;

/// <summary>
/// Provides dependency injection configuration for the Products application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application layer services including MediatR, validators, and the Products API.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The configured service collection for method chaining</returns>
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
        
        services.AddScoped<IProductsAPI, ProductsAPI>();

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, Module>();

        return services;
    }
}