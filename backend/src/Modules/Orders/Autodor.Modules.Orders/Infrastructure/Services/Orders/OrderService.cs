using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Frozen;

namespace Autodor.Modules.Orders.Infrastructure.Services.Orders;

/// <summary>
/// Service for enriching orders with product data and marking exclusions.
/// Handlers decide what to do with IsExcluded flags (filter or show).
/// </summary>
public class OrderService(
    IDistributorsSalesClient distributorsSalesClient,
    OrdersDbContext dbContext,
    IProductsClient productsClient) : IOrderService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var dates = DateTimeUtilities.EachDay(from, to);

        var ordersPerDay = await Task.WhenAll(dates.Select(distributorsSalesClient.GetOrdersAsync));

        var orderDtos = ordersPerDay.SelectMany(x => x).ToList();

        return orderDtos is { Count: > 0 }
            ? await EnrichOrdersAsync(orderDtos, ct)
            : [];
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersAsync(
        IEnumerable<DateTime> dates,
        CancellationToken ct = default)
    {
        var distinctDates = dates.Select(d => d.Date).Distinct().ToList();

        if (distinctDates.Count == 0)
            return [];

        var ordersPerDay = await Task.WhenAll(distinctDates.Select(distributorsSalesClient.GetOrdersAsync));

        var orderDtos = ordersPerDay.SelectMany(x => x).ToList();

        return orderDtos is { Count: > 0 }
            ? await EnrichOrdersAsync(orderDtos, ct)
            : [];
    }

    /// <inheritdoc />
    public async Task<Order?> GetOrderAsync(
        string orderId,
        DateTime date,
        CancellationToken ct = default)
    {
        var orderDto = (await distributorsSalesClient.GetOrdersAsync(date))
            .FirstOrDefault(o => o.Id == orderId);

        if (orderDto is null)
            return null;

        return (await EnrichOrdersAsync([orderDto], ct)).FirstOrDefault();
    }

    /// <summary>
    /// Enriches orders with product data and marks exclusions (does NOT filter).
    /// </summary>
    private async Task<IEnumerable<Order>> EnrichOrdersAsync(
        List<DistributorOrder> orderDtos,
        CancellationToken ct)
    {
        if (orderDtos.Count == 0)
            return [];

        // Filter out orders without IDs upfront
        var validOrders = orderDtos
            .Where(o => !string.IsNullOrWhiteSpace(o.Id))
            .ToList();

        if (validOrders.Count == 0)
            return [];

        var productsTask = productsClient.GetProductsAsync();

        var excludedOrders = (await dbContext.ExcludedOrders
            .AsNoTracking()
            .Select(e => e.Id)
            .ToListAsync(ct)).ToHashSet();

        var excludedItems = (await dbContext.ExcludedOrderItems
            .AsNoTracking()
            .Select(e => new { e.OrderId, e.ItemNumber })
            .ToListAsync(ct))
            .GroupBy(e => e.OrderId)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ItemNumber).ToHashSet());

        var products = await productsTask;

        return validOrders
            .Select(o =>
            {
                var excludedItemsForOrder = excludedItems.GetValueOrDefault(o.Id) ?? [];
                var items = o.Items
                    .Select(item => MapOrderItem(item, excludedItemsForOrder, products))
                    .ToList();

                return new Order
                {
                    Id = o.Id,
                    Number = o.Number,
                    Date = o.Date,
                    Person = o.Person,
                    CustomerNumber = o.CustomerNumber,
                    Items = items,
                    IsExcluded = excludedOrders.Contains(o.Id)
                };
            }
        )
            .ToList();
    }

    /// <summary>
    /// Maps DistributorOrderItem to domain OrderItem with product enrichment and exclusion flag.
    /// </summary>
    private static OrderItem MapOrderItem(
        DistributorOrderItem item,
        HashSet<string> excludedItemNumbers,
        FrozenDictionary<string, Product> products)
    {
        var partNumber = item.PartNumber;

        return new OrderItem
        {
            PartNumber = partNumber,
            ProductName = products.TryGetValue(partNumber, out var product)
                ? product.Name
                : string.Empty,
            Quantity = item.Quantity,
            Price = item.Price,
            IsExcluded = partNumber is not null && excludedItemNumbers.Contains(partNumber)
        };
    }
}
