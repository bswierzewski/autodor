using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;
using BuildingBlocks.Infrastructure.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrder;

public static class GetOrderHandler
{
    [WolverineGet("/orders/{orderId}")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        [AsParameters] GetOrderQuery query,
        IDistributorsSalesService distributorsSalesService,
        CancellationToken ct)
    {
        var orders = await distributorsSalesService.GetOrdersAsync(query.Date);
        var order = orders.FirstOrDefault(o => o.Id == query.OrderId);

        if (order is null)
            return Error.NotFound("Order.NotFound", $"Order with ID '{query.OrderId}' was not found")
                .ToErrorOr<GetOrderResponse>()
                .Problem();

        var response = new GetOrderResponse(
            order.Id!,
            order.Number,
            order.Date,
            order.Person,
            order.CustomerNumber,
            order.Items.Select(i => new OrderItemResponse(
                i.PartNumber,
                i.Quantity,
                i.Price
            )).ToList()
        );

        return Results.Ok(response);
    }
}
