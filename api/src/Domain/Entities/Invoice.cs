namespace Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public int? Number { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime SaleDate { get; set; }
    public DateTime PaymentDue { get; set; }
    public string PaymentMethod { get; set; }
    public string PlaceOfIssue { get; set; }
    public string Notes { get; set; } = string.Empty;

    public int ContractorId { get; set; }
    public Contractor Contractor { get; set; }

    public List<InvoiceItem> Items { get; set; } = new();

    public decimal TotalNet => Items.Sum(i => i.TotalNet);
    public decimal TotalVat => Items.Sum(i => i.VatAmount);
    public decimal TotalGross => Items.Sum(i => i.TotalGross);
}