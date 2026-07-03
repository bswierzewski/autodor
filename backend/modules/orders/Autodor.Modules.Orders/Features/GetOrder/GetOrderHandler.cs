using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Orders.Features.GetOrder;

public static class GetOrderHandler
{
    [Authorize]
    public static async Task<GetOrderResponse> Handle(
        GetOrderCommand command,
        IOrderService orderService,
        CancellationToken ct)
    {
        // Fetch order with exclusions marked (OrderService handles enrichment and marking)
        var order = await orderService.GetOrderAsync(command.Id, command.Date, ct);

        if (order is null)
            throw new NotFoundException($"Nie znaleziono zamówienia o identyfikatorze '{command.Id}'.");

        return new GetOrderResponse(
            order.Id!,
            order.Number,
            order.Date,
            order.Person,
            order.CustomerNumber,
            order.NetAmount,
            order.GrossAmount,
            order.Items.Select(i => new OrderItemResponse(
                i.PartNumber,
                i.ProductDisplayName,
                i.Quantity,
                i.Price,
                IsExcluded: i.IsExcluded
            )).ToList(),
            IsExcluded: order.IsExcluded
        );
    }
}
