using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public static class DeleteContractorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/contractors/{id}", Handle)
            .RequireAuthorization()
            .WithTags("Contractors")
            .WithName("DeleteContractor")
            .WithSummary("Delete contractor");
    }

    private static async Task<IResult> Handle(
        [AsParameters] DeleteContractorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        await bus.InvokeAsync(command, ct);
        return Results.NoContent();
    }
}