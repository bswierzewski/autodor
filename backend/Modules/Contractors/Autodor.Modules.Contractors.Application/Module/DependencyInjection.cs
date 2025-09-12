using System.Reflection;
using BuildingBlocks.Application;
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

        return services;
    }
}