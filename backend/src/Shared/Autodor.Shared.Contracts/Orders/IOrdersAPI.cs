using Autodor.Shared.Contracts.Orders.Dtos;

namespace Autodor.Shared.Contracts.Orders;

public interface IOrdersAPI
{
    /// <summary>
    /// Gets all orders for specified dates
    /// </summary>
    Task<IEnumerable<OrderDto>> GetOrdersByDatesAsync(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders within a date range
    /// </summary>
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all excluded order IDs
    /// </summary>
    Task<IEnumerable<string>> GetExcludedOrderIdsAsync(CancellationToken cancellationToken = default);
}