using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public static class GenerateDeliveryNoteEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/delivery-notes", Handle)
            .RequireAuthorization()
            .WithTags("Orders")
            .WithName("GenerateDeliveryNote")
            .WithSummary("Generate PDF delivery note for an order")
            .Produces<byte[]>(StatusCodes.Status200OK, contentType: "application/pdf");
    }

    private static Task<FileContentHttpResult> Handle(
        GenerateDeliveryNoteCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<FileContentHttpResult>(command, ct);
    }
}