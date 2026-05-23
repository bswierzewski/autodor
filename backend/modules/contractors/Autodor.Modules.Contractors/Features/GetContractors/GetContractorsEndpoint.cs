using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public static class GetContractorsEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/contractors", Handle)
            .RequireAuthorization()
            .WithTags("Contractors")
            .WithName("GetContractors")
            .WithSummary("Get all contractors, optionally filtered by NIPs");
    }

    private static Task<List<GetContractorsResponse>> Handle(
        [AsParameters] GetContractorsCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<List<GetContractorsResponse>>(command, ct);
    }
}