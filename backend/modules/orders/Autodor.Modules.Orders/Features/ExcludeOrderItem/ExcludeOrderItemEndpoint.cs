using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

public static class ExcludeOrderItemEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/api/orders/{id}/items/{itemNumber}", Handle)
            .RequireAuthorization()
            .WithTags("Orders")
            .WithName("UpdateOrderItemExclusion")
            .WithSummary("Include or exclude order item from invoicing");
    }

    private static Task<IResult> Handle(
        string id,
        string itemNumber,
        ExcludeOrderItemCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        command.Id = id;
        command.ItemNumber = itemNumber;
        return bus.InvokeAsync<IResult>(command, ct);
    }
}