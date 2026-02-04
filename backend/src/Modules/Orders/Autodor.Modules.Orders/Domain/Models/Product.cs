namespace Autodor.Modules.Orders.Domain.Models;

/// <summary>
/// Represents a product from Polcar external API.
/// </summary>
public record Product(
    string Number,
    string Name,
    string? EAN13 = null
);
