using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Infrastructure.Services.Orders;

/// <summary>
/// Service for enriching orders with product data and marking exclusions.
/// Does NOT filter exclusions - handlers decide what to do with IsExcluded flags.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders within the specified date range, enriched with product data and with exclusions marked.
    /// Retrieves orders from EVERY day in the range (from to to).
    /// </summary>
    /// <param name="from">Start date (inclusive).</param>
    /// <param name="to">End date (inclusive).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of enriched orders with IsExcluded flags set.</returns>
    Task<IEnumerable<Order>> GetOrdersAsync(DateTime from, DateTime to, CancellationToken ct = default);

    /// <summary>
    /// Gets orders from specific dates only, enriched with product data and with exclusions marked.
    /// More efficient than GetOrdersAsync when dates are sparse (non-continuous).
    /// </summary>
    /// <param name="dates">Specific dates to retrieve orders from.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of enriched orders with IsExcluded flags set.</returns>
    Task<IEnumerable<Order>> GetOrdersAsync(IEnumerable<DateTime> dates, CancellationToken ct = default);

    /// <summary>
    /// Gets a single order by ID and date, enriched with product data and with exclusions marked.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="date">The date of the order.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The enriched order with IsExcluded flags set, or null if not found.</returns>
    Task<Order?> GetOrderAsync(string orderId, DateTime date, CancellationToken ct = default);
}
