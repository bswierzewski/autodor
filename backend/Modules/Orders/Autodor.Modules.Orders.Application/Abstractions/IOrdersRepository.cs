using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Defines the contract for order data access operations.
/// This repository provides specialized query methods for retrieving orders
/// based on business-specific criteria and date-based filtering.
/// </summary>
public interface IOrdersRepository
{
    /// <summary>
    /// Retrieves all orders for a specific date.
    /// Used for daily order processing and reporting operations.
    /// </summary>
    /// <param name="date">The specific date to filter orders by</param>
    /// <returns>A collection of orders matching the specified date</returns>
    Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
    
    /// <summary>
    /// Retrieves all orders within a specified date range.
    /// Used for period-based reporting and batch processing operations.
    /// </summary>
    /// <param name="dateFrom">The start date of the range (inclusive)</param>
    /// <param name="dateTo">The end date of the range (inclusive)</param>
    /// <returns>A collection of orders within the specified date range</returns>
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
}