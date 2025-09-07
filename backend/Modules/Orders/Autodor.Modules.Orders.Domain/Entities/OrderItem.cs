namespace Autodor.Modules.Orders.Domain.Entities;

public class OrderItem
{
    public string Number { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
