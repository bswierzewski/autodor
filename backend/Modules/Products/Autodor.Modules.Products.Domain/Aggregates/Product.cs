using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Products.Domain.Aggregates;

/// <summary>
/// Represents a product entity that serves as the core aggregate root for the Products module.
/// Contains essential product information including identification numbers and descriptive data.
/// </summary>
public class Product : AggregateRoot<int>
{
    /// <summary>
    /// Gets or sets the unique product part number used for identification and catalog references.
    /// This number is typically provided by the external supplier (e.g., Polcar).
    /// </summary>
    public string Number { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the human-readable product name or description.
    /// This field contains the commercial name of the product as provided by the supplier.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the European Article Number (EAN-13) barcode identifier.
    /// This 13-digit code provides global product identification and is used for inventory tracking.
    /// </summary>
    public string EAN13 { get; set; } = string.Empty;
}