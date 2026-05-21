using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public static class GenerateDeliveryNoteEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/delivery-notes", Handle)
            .WithTags("Orders")
            .WithName("GenerateDeliveryNote")
            .WithSummary("Generate PDF delivery note for an order");
    }

    private static Task<IResult> Handle(
        GenerateDeliveryNoteCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<IResult>(command, ct);
    }
}