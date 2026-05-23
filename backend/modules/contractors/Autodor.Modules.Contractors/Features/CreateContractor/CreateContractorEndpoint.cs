using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Contractors.Features.CreateContractor;

public static class CreateContractorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/contractors", Handle)
            .RequireAuthorization()
            .WithTags("Contractors")
            .WithName("CreateContractor")
            .WithSummary("Create a new contractor");
    }

    private static Task<CreateContractorResponse> Handle(
        CreateContractorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<CreateContractorResponse>(command, ct);
    }
}