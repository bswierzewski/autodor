namespace Autodor.Modules.Orders.Domain.Entities;

public class Order
{
    public string Id { get; set; } = null!;
    public string Number { get; set; } = null!;
    public DateTime EntryDate { get; set; }
    public OrderContractor Contractor { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public decimal TotalNetValue => Items.Sum(item => item.NetValue);
    public decimal TotalVatValue => Items.Sum(item => item.VatValue);
    public decimal TotalGrossValue => Items.Sum(item => item.GrossValue);
}

