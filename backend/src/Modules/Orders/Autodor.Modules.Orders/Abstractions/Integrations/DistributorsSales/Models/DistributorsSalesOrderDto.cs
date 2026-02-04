namespace Autodor.Modules.Orders.Abstractions.Integrations.DistributorsSales.Models;

/// <summary>
/// Represents a minimal order data transfer object from distributors sales.
/// </summary>
public record DistributorsSalesOrderDto
{
    /// <summary>
    /// Gets the order identifier (OrderID).
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets the Polcar order number (PolcarOrderNumber).
    /// </summary>
    public string Number { get; init; }

    /// <summary>
    /// Gets the entry date of the order (EntryDate).
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the ordering person name (OrderingPerson).
    /// </summary>
    public string Person { get; init; }

    /// <summary>
    /// Gets the customer number (CustomerNumber).
    /// </summary>
    public string CustomerNumber { get; init; }

    /// <summary>
    /// Gets the list of order items.
    /// </summary>
    public List<DistributorsSalesOrderItemDto> Items { get; init; }
}

/// <summary>
/// Represents a minimal order item data transfer object from distributors sales.
/// </summary>
public record DistributorsSalesOrderItemDto
{
    /// <summary>
    /// Gets the part number (PartNumber).
    /// </summary>
    public string PartNumber { get; init; }

    /// <summary>
    /// Gets the quantity ordered (QuantityOrdered).
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the price (Price).
    /// </summary>
    public decimal Price { get; init; }
}
