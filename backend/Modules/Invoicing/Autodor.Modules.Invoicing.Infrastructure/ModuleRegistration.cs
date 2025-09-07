using Autodor.Modules.Invoicing.Application.Module;
using Autodor.Modules.Invoicing.Infrastructure.Module;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure;

public static class ModuleRegistration
{
    public static IServiceCollection AddInvoicing(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();
        
        return services;
    }
}