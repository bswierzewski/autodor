namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents the contractor information associated with an order.
/// </summary>
public class OrderContractor
{
    /// <summary>
    /// Gets or sets the contractor's unique number identifier.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the contractor's name or company name.
    /// </summary>
    public string Name { get; set; } = null!;
}