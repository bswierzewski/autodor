namespace Autodor.Modules.Orders.Domain.Entities;

public class OrderItem
{
    public string PartNumber { get; set; } = null!;
    public string PartName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
