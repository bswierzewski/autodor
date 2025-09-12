using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Provides methods for retrieving orders from external systems.
/// </summary>
public interface IOrdersRepository
{
    /// <summary>
    /// Retrieves all orders for a specific date.
    /// </summary>
    /// <param name="date">The date for which to retrieve orders.</param>
    /// <returns>A collection of orders for the specified date.</returns>
    Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
    
    /// <summary>
    /// Retrieves all orders within a specified date range.
    /// </summary>
    /// <param name="dateFrom">The start date of the range (inclusive).</param>
    /// <param name="dateTo">The end date of the range (inclusive).</param>
    /// <returns>A collection of orders within the specified date range.</returns>
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
}