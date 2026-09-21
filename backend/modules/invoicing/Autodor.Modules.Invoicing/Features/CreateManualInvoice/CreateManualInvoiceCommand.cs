namespace Autodor.Modules.Invoicing.Features.CreateManualInvoice;

public record CreateManualInvoiceCommand(
    int? InvoiceNumber,
    DateOnly SaleDate,
    DateOnly IssueDate,
    string ContractorNIP,
    IReadOnlyList<ManualInvoiceItem> Items
);

public record ManualInvoiceItem(
    string ItemNumber,
    int Quantity,
    decimal NetUnitPrice
);
