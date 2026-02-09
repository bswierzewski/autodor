using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Dtos;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Services.Caching;
using BuildingBlocks.Kernel.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Services.Orders;

/// <summary>
/// Service for managing orders with business logic applied (exclusions, enrichment).
/// </summary>
public class OrderService(
    IDistributorsSalesClient distributorsSalesService,
    OrdersDbContext dbContext,
    IProductsCache productsCache) : IOrderService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetOrdersAsync(DateTime from, DateTime to, bool includeExcluded = false, CancellationToken ct = default)
    {
        // Generate list of dates in range
        var dates = DateTimeExtensions.EachDay(from, to);

        // Fetch orders in parallel per day
        var ordersTasks = dates.Select(distributorsSalesService.GetOrdersAsync);
        var ordersPerDay = await Task.WhenAll(ordersTasks);

        var allOrderDtos = ordersPerDay.SelectMany(o => o).ToList();

        if (allOrderDtos.Count == 0)
            return [];

        // Map DTOs to domain models with business logic applied
        return await MapToDomainAsync(allOrderDtos, includeExcluded, ct);
    }

    /// <inheritdoc />
    public async Task<Order?> GetOrderAsync(string orderId, DateTime date, bool includeExcluded = false, CancellationToken ct = default)
    {
        // Fetch orders for the specific date
        var orderDtos = await distributorsSalesService.GetOrdersAsync(date);
        var orderDto = orderDtos.FirstOrDefault(o => o.Id == orderId);

        if (orderDto is null)
            return null;

        // Map DTO to domain model with business logic applied
        var processedOrders = await MapToDomainAsync([orderDto], includeExcluded, ct);
        return processedOrders.FirstOrDefault();
    }

    private async Task<IEnumerable<Order>> MapToDomainAsync(List<DistributorOrderDto> orderDtos, bool includeExcluded, CancellationToken ct)
    {
        if (orderDtos.Count == 0)
            return [];

        // 1. Get excluded orders and items
        var excludedOrderIdsList = await dbContext.ExcludedOrders
            .Select(e => e.Id)
            .ToListAsync(ct);
        var excludedOrderIds = excludedOrderIdsList.ToHashSet();

        var orderIds = orderDtos
            .Where(o => !string.IsNullOrWhiteSpace(o.Id))
            .Select(o => o.Id!)
            .ToList();

        var excludedItems = await dbContext.ExcludedOrderItems
            .Where(e => orderIds.Contains(e.OrderId))
            .ToListAsync(ct);

        var excludedItemsSet = excludedItems
            .Select(e => (e.OrderId, e.ItemNumber))
            .ToHashSet();

        // 2. Get all unique part numbers for product enrichment
        var productNumbers = orderDtos
            .SelectMany(o => o.Items)
            .Select(i => i.PartNumber)
            .Where(pn => !string.IsNullOrWhiteSpace(pn))
            .Distinct()
            .ToArray();

        var productsDictionary = productNumbers.Length > 0
            ? (await productsCache.GetByNumbersAsync(productNumbers!)).ToDictionary(p => p.Number, p => p)
            : [];

        // 3. Map DTOs to domain models with all business logic applied in one pass
        var orders = new List<Order>();

        foreach (var dto in orderDtos)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                continue;

            var isOrderExcluded = excludedOrderIds.Contains(dto.Id);

            // Skip excluded orders if not including them
            if (!includeExcluded && isOrderExcluded)
                continue;

            // Map items with enrichment and exclusion logic
            var items = dto.Items
                .Select(itemDto =>
                {
                    var isItemExcluded = !string.IsNullOrWhiteSpace(itemDto.PartNumber) &&
                                        excludedItemsSet.Contains((dto.Id!, itemDto.PartNumber!));

                    // Skip excluded items if not including them
                    if (!includeExcluded && isItemExcluded)
                        return null;

                    // Get product name if available
                    var productName = !string.IsNullOrWhiteSpace(itemDto.PartNumber) &&
                                     productsDictionary.TryGetValue(itemDto.PartNumber!, out var product)
                        ? product.Name
                        : string.Empty;

                    return new OrderItem
                    {
                        PartNumber = itemDto.PartNumber,
                        ProductName = productName,
                        Quantity = itemDto.Quantity,
                        Price = itemDto.Price,
                        IsExcluded = includeExcluded && isItemExcluded
                    };
                })
                .Where(item => item is not null)
                .ToList();

            // Skip orders with no items after filtering (only when not including excluded)
            if (!includeExcluded && items.Count == 0)
                continue;

            orders.Add(new Order
            {
                Id = dto.Id,
                Number = dto.Number,
                Date = dto.Date,
                Person = dto.Person,
                CustomerNumber = dto.CustomerNumber,
                Items = items!,
                IsExcluded = includeExcluded && isOrderExcluded
            });
        }

        return orders;
    }
}
