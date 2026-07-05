namespace Autodor.Modules.Invoicing.Features.CreateInvoice;

public record CreateInvoiceCommand(
    int? InvoiceNumber,
    DateTime SaleDate,
    DateTime IssueDate,
    IEnumerable<DateTime> Dates,
    IEnumerable<string> OrderIds,
    string ContractorNIP
);
