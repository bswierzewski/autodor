namespace Autodor.Modules.Orders.Domain.ValueObjects;

/// <summary>
/// The combination of OrderId and ItemNumber forms a composite identifier.
/// </summary>
public record OrderItemId
{
    public string OrderId { get; }
    public string ItemNumber { get; }

    public OrderItemId(string orderId, string itemNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
        ArgumentException.ThrowIfNullOrWhiteSpace(itemNumber, nameof(itemNumber));

        OrderId = orderId;
        ItemNumber = itemNumber;
    }
}
