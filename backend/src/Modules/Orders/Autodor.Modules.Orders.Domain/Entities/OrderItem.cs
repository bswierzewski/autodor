namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents an individual item within an order, including part details, quantity, and pricing.
/// </summary>
public class OrderItem
{
    private const decimal DefaultVatRate = 0.23m;

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

    /// <summary>
    /// Gets or sets the VAT rate for this item. Defaults to 23%.
    /// </summary>
    public decimal VatRate { get; set; } = DefaultVatRate;

    /// <summary>
    /// Gets the total net value (Price * Quantity).
    /// </summary>
    public decimal NetValue => Price * Quantity;

    /// <summary>
    /// Gets the total VAT value (NetValue * VatRate).
    /// </summary>
    public decimal VatValue => NetValue * VatRate;

    /// <summary>
    /// Gets the total gross value (NetValue + VatValue).
    /// </summary>
    public decimal GrossValue => NetValue + VatValue;
}
