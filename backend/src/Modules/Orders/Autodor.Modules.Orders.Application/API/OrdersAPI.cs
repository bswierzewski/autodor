using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Modules.Orders.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.API;

/// <summary>
/// Implementation of IOrdersAPI that provides order data access for external modules.
/// </summary>
public class OrdersAPI : IOrdersAPI
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrdersDbContext _dbContext;

    public OrdersAPI(IOrdersRepository ordersRepository, IOrdersDbContext dbContext)
    {
        _ordersRepository = ordersRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets all orders for specified dates by querying the repository for each date.
    /// </summary>
    /// <param name="dates">Collection of dates to retrieve orders for</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Collection of orders from all specified dates</returns>
    public async Task<IEnumerable<OrderDto>> GetOrdersByDatesAsync(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default)
    {
        var allOrders = new List<OrderDto>();

        foreach (var date in dates)
        {
            var orders = await _ordersRepository.GetOrdersByDateAsync(date);
            var orderDtos = orders.Select(o => o.ToDto());
            allOrders.AddRange(orderDtos);
        }

        return allOrders;
    }

    /// <summary>
    /// Gets all orders within a specified date range.
    /// </summary>
    /// <param name="dateFrom">Start date of the range (inclusive)</param>
    /// <param name="dateTo">End date of the range (inclusive)</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Collection of orders within the date range</returns>
    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
    {
        var orders = await _ordersRepository.GetOrdersByDateRangeAsync(dateFrom, dateTo);
        return orders.Select(o => o.ToDto());
    }

    /// <summary>
    /// Gets all excluded order IDs directly from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Collection of excluded order IDs</returns>
    public async Task<IEnumerable<string>> GetExcludedOrderIdsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExcludedOrders
            .AsNoTracking()
            .Select(e => e.OrderId)
            .ToListAsync(cancellationToken);
    }

}