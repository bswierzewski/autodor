using System.Reflection;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Autodor.Modules.Orders.Application.API;
using Autodor.Shared.Contracts.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Application.Module;

public static class DependencyInjection
{
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

        // Register external API services
        services.AddScoped<IOrdersAPI, OrdersAPI>();

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, Module>();

        return services;
    }
}