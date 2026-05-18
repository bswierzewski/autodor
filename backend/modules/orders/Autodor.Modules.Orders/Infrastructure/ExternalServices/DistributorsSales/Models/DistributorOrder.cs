namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Models;

/// <summary>
/// Represents an order from the Distributors Sales API.
/// </summary>
public record DistributorOrder
{
    /// <summary>
    /// Gets the order identifier (OrderID).
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the Polcar order number (PolcarOrderNumber).
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// Gets the entry date of the order (EntryDate).
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the ordering person name (OrderingPerson).
    /// </summary>
    public required string Person { get; init; }

    /// <summary>
    /// Gets the customer number (CustomerNumber).
    /// </summary>
    public required string CustomerNumber { get; init; }

    /// <summary>
    /// Gets the list of order items.
    /// </summary>
    public List<DistributorOrderItem> Items { get; init; } = [];
}

/// <summary>
/// Represents an order item from the Distributors Sales API.
/// </summary>
public record DistributorOrderItem
{
    /// <summary>
    /// Gets the part number (PartNumber).
    /// </summary>
    public required string PartNumber { get; init; }

    /// <summary>
    /// Gets the quantity ordered (QuantityOrdered).
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the price (Price).
    /// </summary>
    public decimal Price { get; init; }
}
