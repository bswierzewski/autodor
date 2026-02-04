using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.ServiceReference;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Extensions;

/// <summary>
/// Extension methods for mapping between DistributorsSales SOAP response types and domain models.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Converts a collection of DistributorSalesOrderResponse to a collection of PolcarOrder.
    /// </summary>
    public static IEnumerable<Order> ToDto(this IEnumerable<DistributorSalesOrderResponse> source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return source
            .Where(item => item is not null)
            .Select(item => item.ToDto());
    }

    /// <summary>
    /// Converts DistributorSalesOrderItemResponse to PolcarOrderItem.
    /// </summary>
    public static OrderItem ToDto(this DistributorSalesOrderItemResponse source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return new OrderItem
        {
            PartNumber = source.PartNumber ?? string.Empty,
            Quantity = source.QuantityOrdered,
            Price = source.Price
        };
    }

    /// <summary>
    /// Converts DistributorSalesOrderResponse to PolcarOrder.
    /// </summary>
    public static Order ToDto(this DistributorSalesOrderResponse source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return new Order
        {
            Id = source.OrderID ?? string.Empty,
            Number = source.PolcarOrderNumber ?? string.Empty,
            Date = source.EntryDate,
            Person = source.OrderingPerson ?? string.Empty,
            CustomerNumber = source.CustomerNumber ?? string.Empty,
            Items = source.OrderedItemsResponse?
                .Where(item => item is not null)
                .Select(item => item.ToDto()).ToList() ?? []
        };
    }
}
