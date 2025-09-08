namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents a customer order received from external systems (e.g., Polcar SOAP service).
/// Contains essential order information including contractor details and ordered items.
/// This entity serves as the primary container for order data in the business domain.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique order identifier from the external system.
    /// This ID is used for tracking and correlation with the originating system.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the human-readable order number.
    /// This number is typically displayed to users and used for order identification in business processes.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the order was initially created or entered into the system.
    /// Used for order processing prioritization and audit trails.
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the contractor information associated with this order.
    /// Contains customer identification and contact details for order fulfillment.
    /// </summary>
    public OrderContractor Contractor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of items included in this order.
    /// Each item represents a specific product with quantity and pricing information.
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

