using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.Services.Caching;

namespace Autodor.Modules.Orders.Infrastructure.Services.OrderEnrichment;

/// <summary>
/// Service for enriching orders with additional product information.
/// </summary>
public class OrderEnrichmentService(IProductsCache productsCache) : IOrderEnrichmentService
{
    /// <inheritdoc />
    public async Task EnrichWithProductNamesAsync(Order order, CancellationToken ct = default)
        => await EnrichWithProductNamesAsync([order], ct);

    /// <inheritdoc />
    public async Task EnrichWithProductNamesAsync(IEnumerable<Order> orders, CancellationToken ct = default)
    {
        var ordersList = orders.ToList();

        // Collect all unique part numbers from all orders
        var productNumbers = ordersList
            .SelectMany(o => o.Items)
            .Select(i => i.PartNumber)
            .Where(pn => !string.IsNullOrWhiteSpace(pn))
            .Distinct()
            .ToArray();

        if (productNumbers.Length == 0)
            return;

        // Fetch all products at once
        var products = await productsCache.GetByNumbersAsync(productNumbers!);
        var productsDictionary = products.ToDictionary(p => p.Number, p => p);

        // Enrich all order items by creating new instances with ProductName
        foreach (var order in ordersList)
        {
            var enrichedItems = order.Items.Select(item =>
            {
                if (string.IsNullOrWhiteSpace(item.PartNumber))
                    return item;

                if (productsDictionary.TryGetValue(item.PartNumber, out var product))
                    return item with { ProductName = product.Name };

                return item;
            }).ToList();

            order.Items.Clear();
            order.Items.AddRange(enrichedItems);
        }
    }
}
