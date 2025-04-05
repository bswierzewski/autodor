namespace Domain.Entities;

public class Order
{
    public DateTime Date { get; set; }
    public string Id { get; set; }
    public string Number { get; set; }
    public string Person { get; set; }
    public string CustomerNumber { get; set; }
    public IEnumerable<OrderItem> Items { get; set; } = Enumerable.Empty<OrderItem>();
}
