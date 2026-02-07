using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrdersHandler
{
    [WolverineGet("/orders")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        [AsParameters] GetOrdersQuery query,
        IDistributorsSalesService distributorsSalesService,
        CancellationToken ct)
    {
        var allOrders = new List<Order>();

        // Fetch orders for each day in the date range
        for (var date = query.From.Date; date <= query.To.Date; date = date.AddDays(1))
        {
            var ordersForDate = await distributorsSalesService.GetOrdersAsync(date);
            allOrders.AddRange(ordersForDate);
        }

        var response = new GetOrdersResponse(
            allOrders
                .Select(o => new OrderSummaryResponse(
                    o.Id!,
                    o.Number,
                    o.Date,
                    o.Person,
                    o.CustomerNumber,
                    o.Items.Count,
                    o.Items.Sum(i => i.Price * i.Quantity)
                ))
                .ToList()
        );

        return Results.Ok(response);
    }
}
