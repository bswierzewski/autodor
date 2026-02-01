using Autodor.Modules.Orders.Domain.ValueObjects;
using BuildingBlocks.Kernel.Abstractions;
using BuildingBlocks.Kernel.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrderItem : Entity<OrderItemId>, IAggregateRoot
{
    private ExcludedOrderItem() { }

    public ExcludedOrderItem(OrderItemId itemId)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        Id = itemId;
    }
}
