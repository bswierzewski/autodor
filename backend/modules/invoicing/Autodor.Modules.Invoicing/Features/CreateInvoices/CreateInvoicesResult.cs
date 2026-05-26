namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public record CreateInvoicesResult(IReadOnlyList<InvoiceSummaryEntry> Details)
{
    public int InvoicesCreated => Details.Count(d => d.Success);
    public int InvoicesSkipped => Details.Count(d => !d.Success);
}

public record InvoiceSummaryEntry(
    string ContractorNip,
    string ContractorName,
    int ItemCount,
    bool Success,
    string? ErrorMessage
);
