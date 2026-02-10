namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

public record InvoiceItem
{
    public required string Name { get; init; }
    public string Unit { get; init; } = "szt";
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public decimal VatRate { get; init; } = 0.23m;
    public string VatType { get; init; } = "PRC";
    public decimal Discount { get; init; } = 0;
    public string PKWiU { get; init; } = "";
    public string GTU { get; init; } = "";
    public decimal NetAmount => UnitPrice * Quantity * (1 - Discount / 100);
    public decimal VatAmount => NetAmount * VatRate;
    public decimal TotalNet => NetAmount;
    public decimal TotalGross => NetAmount + VatAmount;
};