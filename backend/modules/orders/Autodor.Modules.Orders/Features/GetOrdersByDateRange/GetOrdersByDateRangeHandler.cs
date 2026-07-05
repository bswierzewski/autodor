using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;

namespace Autodor.Modules.Orders.Features.GetOrdersByDateRange;

public static class GetOrdersByDateRangeHandler
{
    public static async Task<IEnumerable<OrderDto>> Handle(
        GetOrdersByDateRangeQuery query,
        IOrderService orderService,
        CancellationToken ct)
    {
        var orders = await orderService.GetOrdersAsync(query.DateFrom, query.DateTo, ct);

        return orders
            // Filter out excluded orders
            .Where(o => !o.IsExcluded)
            .Select(o => new OrderDto
            {
                Id = o.Id ?? string.Empty,
                Number = o.Number ?? string.Empty,
                Date = o.Date,
                Person = o.Person ?? string.Empty,
                CustomerNumber = o.CustomerNumber ?? string.Empty,
                // Filter out excluded items
                Items = [.. o.Items
                    .Where(i => !i.IsExcluded)
                    .Select(i => new OrderItemDto
                    {
                        Name = i.ProductDisplayName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    })]
            })
            // Remove orders that have no items after filtering
            .Where(o => o.Items.Count != 0);
    }
}
