using Autodor.Modules.Orders.Infrastructure.Services.Orders;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersHandler
{
    public static async Task<GetOrdersResponse> Handle(
        GetOrdersCommand command,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch orders with exclusions marked (OrderService handles enrichment and marking)
        var orders = await orderService.GetOrdersAsync(command.From, command.To, ct);

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
