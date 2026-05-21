using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public static class UpdateContractorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/contractors/{id}", Handle)
            .WithTags("Contractors")
            .WithName("UpdateContractor")
            .WithSummary("Update contractor details");
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateContractorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        command.Id = id;
        await bus.InvokeAsync(command, ct);
        return Results.NoContent();
    }
}