using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors;

public sealed class ContractorsModule : IModule
{
    public string Name => "Contractors";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres<ContractorsDbContext>(ContractorsDbContext.Schema);
    }
}
