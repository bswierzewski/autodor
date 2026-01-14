using Autodor.Modules.Contractors.Domain;
using Autodor.Modules.Contractors.Infrastructure.Database;
using Autodor.Shared.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors;

public static class ContractorsModuleExtensions
{
    public static IServiceCollection AddContractorsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Module.Name)
            .AddOptions(opts => { })
            .AddPostgres<ContractorsDbContext>();

        return services;
    }
}
