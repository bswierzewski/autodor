using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersHandler
{
    [WolverineGet("/api/orders")]
    [Tags("Orders")]
    [EndpointName("GetOrders")]
    [EndpointSummary("Get all orders within date range")]
    public static async Task<GetOrdersResponse> Handle(
        [AsParameters] GetOrdersQuery query,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch orders with exclusions marked (OrderService handles enrichment and marking)
        var orders = await orderService.GetOrdersAsync(query.From, query.To, ct);

        return new GetOrdersResponse(
            orders
                .Select(o => new OrderSummaryResponse(
                    o.Id!,
                    o.Number,
                    o.Date,
                    o.Person,
                    o.CustomerNumber,
                    o.Items.Count,
                    o.Items.Sum(i => i.Price * i.Quantity),
                    IsExcluded: o.IsExcluded,
                    ExcludedItemsCount: o.Items.Count(i => i.IsExcluded)
                ))
                .ToList()
        );
    }
}
