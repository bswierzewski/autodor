using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors;

public static class ContractorsModuleExtensions
{
    public static IServiceCollection AddContractorsModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
