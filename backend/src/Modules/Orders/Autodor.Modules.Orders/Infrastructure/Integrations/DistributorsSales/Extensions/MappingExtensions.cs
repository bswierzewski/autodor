using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Dtos;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.ServiceReference;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Extensions;

/// <summary>
/// Extension methods for mapping between DistributorsSales SOAP response types and DTOs.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Converts a collection of DistributorSalesOrderResponse to a collection of DistributorOrderDto.
    /// </summary>
    public static IEnumerable<DistributorOrderDto> ToDto(this IEnumerable<DistributorSalesOrderResponse> source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return source
            .Where(item => item is not null)
            .Select(item => item.ToDto());
    }

    /// <summary>
    /// Converts DistributorSalesOrderItemResponse to DistributorOrderItemDto.
    /// </summary>
    public static DistributorOrderItemDto ToDto(this DistributorSalesOrderItemResponse source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return new DistributorOrderItemDto
        {
            PartNumber = source.PartNumber ?? string.Empty,
            Quantity = source.QuantityOrdered,
            Price = source.Price
        };
    }

    /// <summary>
    /// Converts DistributorSalesOrderResponse to DistributorOrderDto.
    /// </summary>
    public static DistributorOrderDto ToDto(this DistributorSalesOrderResponse source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        return new DistributorOrderDto
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
