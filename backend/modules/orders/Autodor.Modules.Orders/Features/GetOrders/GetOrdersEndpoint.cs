using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/orders", Handle)
            .RequireAuthorization()
            .WithTags("Orders")
            .WithName("GetOrders")
            .WithSummary("Get all orders within date range");
    }

    private static Task<GetOrdersResponse> Handle(
        [AsParameters] GetOrdersCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<GetOrdersResponse>(command, ct);
    }
}
