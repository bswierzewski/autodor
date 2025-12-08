namespace Autodor.Modules.Products.Domain.Entities;

/// <summary>
/// Represents a product entity containing essential product information from external catalog.
/// This is a read-only entity loaded from Polcar API and stored in-memory.
/// </summary>
public class Product
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