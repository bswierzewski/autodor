using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors;

public static class ContractorsModule
{
    public static readonly string Name = "Contractors";

    public static IServiceCollection AddContractorsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Name)
            .AddPostgres<ContractorsDbContext>()
            .Build();

        return services;
    }
}
