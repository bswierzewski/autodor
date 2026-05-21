using Autodor.Modules.Contractors.Features.CreateContractor;
using Autodor.Modules.Contractors.Features.DeleteContractor;
using Autodor.Modules.Contractors.Features.GetContractor;
using Autodor.Modules.Contractors.Features.GetContractors;
using Autodor.Modules.Contractors.Features.UpdateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors;

public sealed class ContractorsModule : IEndpointModule
{
    public string Name => "Contractors";

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        CreateContractorEndpoint.Map(endpoints);
        DeleteContractorEndpoint.Map(endpoints);
        GetContractorEndpoint.Map(endpoints);
        GetContractorsEndpoint.Map(endpoints);
        UpdateContractorEndpoint.Map(endpoints);
    }

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres<ContractorsDbContext>(ContractorsDbContext.Schema);
    }
}
