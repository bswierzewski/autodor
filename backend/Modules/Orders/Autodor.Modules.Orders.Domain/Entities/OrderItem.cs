namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents an individual item within an order, including part details, quantity, and pricing.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the identifier of the order this item belongs to.
    /// </summary>
    public string OrderId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the part number for this order item.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity ordered for this item.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of this item.
    /// </summary>
    public decimal Price { get; set; }
}
