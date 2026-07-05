namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;

/// <summary>
/// Represents a product item from the external Products API.
/// </summary>
public record Product
{
    /// <summary>
    /// Gets the unique product number identifier.
    /// </summary>
    public string Number { get; init; } = string.Empty;

    /// <summary>
    /// Gets the display name of the product part.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the EAN13 barcode for the product.
    /// </summary>
    public string EAN13 { get; init; } = string.Empty;
}
