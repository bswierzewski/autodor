using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public static class ExcludeOrderEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/api/orders/{id}", Handle)
            .RequireAuthorization()
            .WithTags("Orders")
            .WithName("UpdateOrderExclusion")
            .WithSummary("Include or exclude order from invoicing");
    }

    private static Task<IResult> Handle(
        string id,
        ExcludeOrderCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        command.Id = id;
        return bus.InvokeAsync<IResult>(command, ct);
    }
}