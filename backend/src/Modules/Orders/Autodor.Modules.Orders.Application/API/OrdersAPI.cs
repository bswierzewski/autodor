using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Modules.Orders.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.API;

public class OrdersAPI(IOrdersRepository ordersRepository, IOrdersDbContext dbContext) : IOrdersAPI
{

    /// <summary>
    /// Gets all orders for specified dates by querying the repository for each date.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByDatesAsync(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var allOrders = new List<OrderDto>();

        foreach (var date in dates)
        {
            var orders = await ordersRepository.GetOrdersByDateAsync(date);
            var orderDtos = orders.Select(o => o.ToDto());
            allOrders.AddRange(orderDtos);
        }

        return allOrders;
    }

    /// <summary>
    /// Gets all orders within a specified date range.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
    {
        var orders = await ordersRepository.GetOrdersByDateRangeAsync(dateFrom, dateTo);
        return orders.Select(o => o.ToDto());
    }

    /// <summary>
    /// Gets all excluded order IDs directly from the database.
    /// </summary>
    public async Task<IEnumerable<string>> GetExcludedOrderIdsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ExcludedOrders
            .AsNoTracking()
            .Select(e => e.OrderId)
            .ToListAsync(cancellationToken);
    }
}