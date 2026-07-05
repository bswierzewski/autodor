using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.GetOrder;

public static class GetOrderEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/orders/{id}", Handle)
            .RequireAuthorization()
            .WithTags("Orders")
            .WithName("GetOrder")
            .WithSummary("Get order by ID and date");
    }

    private static Task<GetOrderResponse> Handle(
        [AsParameters] GetOrderCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<GetOrderResponse>(command, ct);
    }
}
