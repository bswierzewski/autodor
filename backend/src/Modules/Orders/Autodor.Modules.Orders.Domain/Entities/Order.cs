namespace Autodor.Modules.Orders.Domain.Entities;

/// <summary>
/// Represents a sales order from an external system containing contractor information and ordered items.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the order number as provided by the external system.
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date when the order was entered into the system.
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the contractor who placed the order.
    /// </summary>
    public OrderContractor Contractor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of items included in this order.
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Gets the total net value of all items in the order.
    /// </summary>
    public decimal TotalNetValue => Items.Sum(item => item.NetValue);

    /// <summary>
    /// Gets the total VAT value of all items in the order.
    /// </summary>
    public decimal TotalVatValue => Items.Sum(item => item.VatValue);

    /// <summary>
    /// Gets the total gross value of all items in the order.
    /// </summary>
    public decimal TotalGrossValue => Items.Sum(item => item.GrossValue);
}

