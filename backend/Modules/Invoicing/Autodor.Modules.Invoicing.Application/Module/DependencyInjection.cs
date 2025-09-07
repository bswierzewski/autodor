using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Application.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}