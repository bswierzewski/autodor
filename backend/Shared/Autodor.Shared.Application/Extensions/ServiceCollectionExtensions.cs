using Autodor.Shared.Application.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Autodor.Shared.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedApplicationBehaviors(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
    
    public static IServiceCollection AddSharedApplicationLogging(this IServiceCollection services)
    {
        services.AddScoped(typeof(LoggingBehaviour<>));
        return services;
    }
}