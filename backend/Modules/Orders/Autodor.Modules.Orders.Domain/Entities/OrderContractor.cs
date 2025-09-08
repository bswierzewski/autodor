namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents contractor information associated with an order.
/// Contains essential customer details needed for order processing and fulfillment.
/// This is a simplified contractor representation specific to the Orders domain context.
/// </summary>
public class OrderContractor
{
    /// <summary>
    /// Gets or sets the contractor's unique identification number.
    /// This is typically the customer number from the external system (e.g., Polcar)
    /// and serves as the primary identifier for business operations.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the person or entity placing the order.
    /// This represents the ordering party and is used for contact and delivery purposes.
    /// </summary>
    public string Name { get; set; } = null!;
}