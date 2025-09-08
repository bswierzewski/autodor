namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents a single line item within an order.
/// Contains product information, quantities, and pricing details for order processing.
/// This entity maintains the relationship between orders and specific products being ordered.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the identifier of the parent order.
    /// Used to establish the relationship between the item and its containing order.
    /// </summary>
    public string OrderId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product number or part number.
    /// This is the unique identifier for the specific product being ordered,
    /// typically provided by the supplier's catalog system.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity of the product being ordered.
    /// Represents the number of units requested by the customer.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// This is the cost per individual item and is used for order total calculations.
    /// </summary>
    public decimal Price { get; set; }
}
