namespace Autodor.Modules.Orders.Domain.Entities;

public class OrderItem
{
    public string OrderId { get; set; } = null!;
    public string Number { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal VatRate { get; set; } = 0.23m;
    public decimal NetValue => Price * Quantity;
    public decimal VatValue => NetValue * VatRate;
    public decimal GrossValue => NetValue + VatValue;
}
