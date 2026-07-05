namespace Autodor.Modules.Orders.Contracts.Models;

/// <summary>
/// Data Transfer Object for OrderItem entity
/// Used for inter-module communication
/// NOTE: Excluded items are already filtered out by Orders module
/// </summary>
public record OrderItemDto
{
    public string Name { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}
