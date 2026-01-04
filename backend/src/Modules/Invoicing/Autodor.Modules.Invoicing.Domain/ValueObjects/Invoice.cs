namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

public record Invoice
{
    public int? Number { get; init; }
    public required DateTime IssueDate { get; init; }
    public required DateTime SaleDate { get; init; }
    public DateTime PaymentDue { get; init; }
    public string PaymentMethod { get; init; } = "transfer";
    public string PlaceOfIssue { get; init; } = "Katowice";
    public string Notes { get; init; } = "";
    public required Contractor Contractor { get; init; }
    public required IReadOnlyList<InvoiceItem> Items { get; init; }
    public decimal TotalNet => Items.Sum(i => i.TotalNet);
    public decimal TotalVat => Items.Sum(i => i.VatAmount);
    public decimal TotalGross => Items.Sum(i => i.TotalGross);
};