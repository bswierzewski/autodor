using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar;

internal static class MappingExtensions
{
    internal static Order MapToOrder(this DistributorSalesOrderResponse response)
    {
        return new Order
        {
            EntryDate = response.EntryDate,
            Id = response.OrderID,
            Number = response.PolcarOrderNumber,

            Contractor = new OrderContractor
            {
                Name = response.OrderingPerson,
                Number = response.CustomerNumber,
            },

            Items = response.OrderedItemsResponse?.Select(MapToOrderItem).ToList() ?? []
        };
    }

    internal static Order MapToOrder(this SalesOrderResponse response)
    {
        return new Order
        {
            EntryDate = DateTime.UtcNow,
            Id = response.OrderID,
            Number = response.PolcarOrderNumber,

            Contractor = new OrderContractor
            {
                Name = response.OrderingPerson,
                Number = response.CustomerNumber,
            },

            Items = response.OrderedItemsResponse?.Select(item => item.MapToOrderItem(response.OrderID)).ToList() ?? []
        };
    }

    private static OrderItem MapToOrderItem(this DistributorSalesOrderItemResponse response)
    {
        return new OrderItem
        {
            OrderId = response.OrderId,
            Number = response.PartNumber,

            Quantity = response.QuantityOrdered,
            Price = response.Price
        };
    }

    private static OrderItem MapToOrderItem(this SalesOrderItemResponse response, string orderId)
    {
        return new OrderItem
        {
            OrderId = orderId,
            Number = response.PolcarPartNumber ?? response.CustomerPartNumber,

            Quantity = response.QuantityOrdered,
            Price = response.DistributorPrice
        };
    }
}
