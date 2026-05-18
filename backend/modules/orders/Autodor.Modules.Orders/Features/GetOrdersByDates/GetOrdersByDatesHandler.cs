using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;

namespace Autodor.Modules.Orders.Features.GetOrdersByDates;

public static class GetOrdersByDatesHandler
{
    public static async Task<IEnumerable<OrderDto>> Handle(
        GetOrdersByDatesQuery query,
        IOrderService orderService,
        CancellationToken ct)
    {
        if (!query.Dates.Any())
            return [];

        // Use GetOrdersByDatesAsync - only fetches orders from specified dates (more efficient for sparse dates)
        var orders = await orderService.GetOrdersAsync(query.Dates, ct);

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
                Items = o.Items
                    .Where(i => !i.IsExcluded)
                    .Select(i => new OrderItemDto
                    {
                        Name = i.ProductDisplayName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
            })
            // Remove orders that have no items after filtering
            .Where(o => o.Items.Any());
    }
}
