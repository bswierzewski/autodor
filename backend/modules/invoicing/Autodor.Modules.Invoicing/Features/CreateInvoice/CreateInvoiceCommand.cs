namespace Autodor.Modules.Invoicing.Features.CreateInvoice;

public record CreateInvoiceCommand(
    int? InvoiceNumber,
    DateOnly SaleDate,
    DateOnly IssueDate,
    IEnumerable<DateTime> Dates,
    IEnumerable<string> OrderIds,
    string ContractorNIP
);
