using Autodor.Modules.Orders.Domain.Models;

namespace Autodor.Modules.Orders.Infrastructure.Services.OrderEnrichment;

/// <summary>
/// Service for enriching orders with additional product information.
/// </summary>
public interface IOrderEnrichmentService
{
    /// <summary>
    /// Enriches order items with product names.
    /// Modifies PartNumber to include product name: "ProductName (PartNumber)".
    /// </summary>
    /// <param name="order">The order to enrich.</param>
    /// <param name="ct">Cancellation token.</param>
    Task EnrichWithProductNamesAsync(Order order, CancellationToken ct = default);

    /// <summary>
    /// Enriches multiple orders with product names.
    /// Modifies PartNumber to include product name: "ProductName (PartNumber)".
    /// </summary>
    /// <param name="orders">The orders to enrich.</param>
    /// <param name="ct">Cancellation token.</param>
    Task EnrichWithProductNamesAsync(IEnumerable<Order> orders, CancellationToken ct = default);
}
