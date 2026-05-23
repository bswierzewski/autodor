using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Contractors.Features.GetContractor;

public static class GetContractorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/contractors/{id}", Handle)
            .RequireAuthorization()
            .WithTags("Contractors")
            .WithName("GetContractor")
            .WithSummary("Get contractor by ID");
    }

    private static Task<GetContractorResponse> Handle(
        [AsParameters] GetContractorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<GetContractorResponse>(command, ct);
    }
}