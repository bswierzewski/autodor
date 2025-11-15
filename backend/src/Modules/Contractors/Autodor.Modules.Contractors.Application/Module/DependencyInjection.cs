using System.Reflection;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using Autodor.Shared.Contracts.Contractors;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Application.Module;

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
        services.AddScoped<IContractorsAPI, ContractorsAPI>();

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, Module>();

        return services;
    }
}