using Autodor.Modules.Orders.Domain.Models;

namespace Autodor.Modules.Orders.Infrastructure.Services.Orders;

/// <summary>
/// Service for managing orders with business logic applied (exclusions, enrichment).
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders within the specified date range with exclusions applied and enriched with product data.
    /// </summary>
    /// <param name="from">Start date (inclusive).</param>
    /// <param name="to">End date (inclusive).</param>
    /// <param name="includeExcluded">If true, includes excluded orders with IsExcluded flags set; if false, filters them out.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of processed orders.</returns>
    Task<IEnumerable<Order>> GetOrdersAsync(DateTime from, DateTime to, bool includeExcluded = false, CancellationToken ct = default);

    /// <summary>
    /// Gets a single order by ID and date with exclusions applied and enriched with product data.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="date">The date of the order.</param>
    /// <param name="includeExcluded">If true, includes the order even if excluded with IsExcluded flag set; if false, returns null if excluded.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The processed order, or null if not found or fully excluded (when includeExcluded is false).</returns>
    Task<Order?> GetOrderAsync(string orderId, DateTime date, bool includeExcluded = false, CancellationToken ct = default);
}
