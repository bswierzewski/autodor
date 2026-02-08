namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products.Dtos;

/// <summary>
/// DTO representing a product from the Products API.
/// </summary>
public record ProductDto
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
