using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Products.Domain.Aggregates;

/// <summary>
/// Represents a product aggregate containing essential product information for catalog management.
/// </summary>
public class Product : AggregateRoot<int>
{
    /// <summary>
    /// Gets or sets the unique product number used for identification.
    /// </summary>
    public string Number { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the EAN13 barcode for the product.
    /// </summary>
    public string EAN13 { get; set; } = string.Empty;
}