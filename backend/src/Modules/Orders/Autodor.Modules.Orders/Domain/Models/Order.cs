namespace Autodor.Modules.Orders.Domain.Models;

/// <summary>
/// Represents an order from Polcar distributors sales.
/// </summary>
public record Order
{
    /// <summary>
    /// Gets the order identifier (OrderID).
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the Polcar order number (PolcarOrderNumber).
    /// </summary>
    public string? Number { get; init; }

    /// <summary>
    /// Gets the entry date of the order (EntryDate).
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the ordering person name (OrderingPerson).
    /// </summary>
    public string? Person { get; init; }

    /// <summary>
    /// Gets the customer number (CustomerNumber).
    /// </summary>
    public string? CustomerNumber { get; init; }

    /// <summary>
    /// Gets the list of order items.
    /// </summary>
    public List<OrderItem> Items { get; init; } = [];
}

/// <summary>
/// Represents an order item from Polcar distributors sales.
/// </summary>
public record OrderItem
{
    /// <summary>
    /// Gets the part number (PartNumber).
    /// </summary>
    public string? PartNumber { get; init; }

    /// <summary>
    /// Gets the quantity ordered (QuantityOrdered).
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the price (Price).
    /// </summary>
    public decimal Price { get; init; }
}
