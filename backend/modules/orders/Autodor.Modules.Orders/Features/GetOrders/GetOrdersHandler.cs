using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersHandler
{
    [Authorize]
    public static async Task<GetOrdersResponse> Handle(
        GetOrdersCommand command,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch orders with exclusions marked (OrderService handles enrichment and marking)
        var orders = await orderService.GetOrdersAsync(command.From, command.To, ct);

        return new GetOrdersResponse(
            [.. orders
                .OrderByDescending(o => o.Date)
                .Select(o => new OrderSummaryResponse(
                    o.Id!,
                    o.Number,
                    o.Date,
                    o.Person,
                    o.CustomerNumber,
                    o.Items.Count,
                    o.NetAmount,
                    o.GrossAmount,
                    IsExcluded: o.IsExcluded,
                    ExcludedItemsCount: o.Items.Count(i => i.IsExcluded)
                ))]
        );
    }
}
