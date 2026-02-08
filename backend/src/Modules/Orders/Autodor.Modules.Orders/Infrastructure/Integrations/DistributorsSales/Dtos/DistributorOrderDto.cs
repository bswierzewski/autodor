namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Dtos;

/// <summary>
/// DTO representing an order from the distributors sales API.
/// </summary>
public record DistributorOrderDto
{
    /// <summary>
    /// Gets the order identifier (OrderID).
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Polcar order number (PolcarOrderNumber).
    /// </summary>
    public string Number { get; init; } = string.Empty;

    /// <summary>
    /// Gets the entry date of the order (EntryDate).
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the ordering person name (OrderingPerson).
    /// </summary>
    public string Person { get; init; } = string.Empty;

    /// <summary>
    /// Gets the customer number (CustomerNumber).
    /// </summary>
    public string CustomerNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the list of order items.
    /// </summary>
    public List<DistributorOrderItemDto> Items { get; init; } = [];
}

/// <summary>
/// DTO representing an order item from the distributors sales API.
/// </summary>
public record DistributorOrderItemDto
{
    /// <summary>
    /// Gets the part number (PartNumber).
    /// </summary>
    public string PartNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the quantity ordered (QuantityOrdered).
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets the price (Price).
    /// </summary>
    public decimal Price { get; init; }
}
