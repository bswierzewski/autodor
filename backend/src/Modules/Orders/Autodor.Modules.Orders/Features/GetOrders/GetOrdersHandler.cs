using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersHandler
{
    [WolverineGet("/orders")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        [AsParameters] GetOrdersQuery query,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch orders with exclusions marked (includeExcluded = true)
        var orders = await orderService.GetOrdersAsync(query.From, query.To, includeExcluded: true, ct);

        var response = new GetOrdersResponse(
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

        return Results.Ok(response);
    }
}
