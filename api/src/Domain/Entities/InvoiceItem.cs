namespace Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public string Name { get; set; }
    public string Unit { get; set; } = "sztuk";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; } = 0.23m; // 23% VAT
    public string VatType { get; set; }
    public decimal Discount { get; set; }
    public string PKWiU { get; set; } = string.Empty;
    public string GTU { get; set; } = string.Empty;

    public decimal NetAmount => UnitPrice * Quantity * (1 - Discount / 100);
    public decimal VatAmount => NetAmount * VatRate;
    public decimal TotalNet => NetAmount;
    public decimal TotalGross => NetAmount + VatAmount;
}