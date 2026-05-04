using Autodor.Modules.Orders.Domain.ValueObjects;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrderItem
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
