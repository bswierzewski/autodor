using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrder;

public static class GetOrderHandler
{
    [WolverineGet("/api/orders/{id}")]
    [Tags("Orders")]
    [EndpointName("GetOrder")]
    [EndpointSummary("Get order by ID and date")]
    public static async Task<IResult> Handle(
        [AsParameters] GetOrderQuery query,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch order with exclusions marked (OrderService handles enrichment and marking)
        var order = await orderService.GetOrderAsync(query.OrderId, query.Date, ct);

        if (order is null)
            return Error.NotFound("Order.NotFound", $"Order with ID '{query.OrderId}' was not found").Problem();

        var response = new GetOrderResponse(
            order.Id!,
            order.Number,
            order.Date,
            order.Person,
            order.CustomerNumber,
            order.Items.Select(i => new OrderItemResponse(
                i.ProductDisplayName,
                i.Quantity,
                i.Price,
                IsExcluded: i.IsExcluded
            )).ToList(),
            IsExcluded: order.IsExcluded
        );

        return Results.Ok(response);
    }
}
