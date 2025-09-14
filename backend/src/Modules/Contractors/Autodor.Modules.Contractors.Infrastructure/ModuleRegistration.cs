using Autodor.Modules.Contractors.Application.Module;
using Autodor.Modules.Contractors.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Infrastructure;

public static class ModuleRegistration
{
    public static IServiceCollection AddContractors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        
        services.AddInfrastructure(configuration);

        return services;
    }
}