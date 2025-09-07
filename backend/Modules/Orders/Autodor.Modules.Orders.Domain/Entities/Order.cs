namespace Autodor.Modules.Orders.Domain.Entities;

public class Order
{
    public DateTime Date { get; set; }
    public string Id { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string Contractor { get; set; } = null!;
    public IEnumerable<OrderItem> Items { get; set; } = Enumerable.Empty<OrderItem>();
}

