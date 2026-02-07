using Autodor.Modules.Orders.Domain.ValueObjects;
using BuildingBlocks.Kernel.Abstractions;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrderItem : IAggregateRoot
{
    public string OrderId { get; private set; } = default!;
    public string ItemNumber { get; private set; } = default!;

    private ExcludedOrderItem() { }

    public ExcludedOrderItem(OrderItemId itemId)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        OrderId = itemId.OrderId;
        ItemNumber = itemId.ItemNumber;
    }
}
