namespace Autodor.Modules.Orders.Contracts.Models;

/// <summary>
/// Data Transfer Object for Order entity
/// Used for inter-module communication
/// NOTE: Excluded orders and items are already filtered out by Orders module
/// </summary>
public record OrderDto
{
    public string Id { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Person { get; init; } = string.Empty;
    public string CustomerNumber { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = [];
}
