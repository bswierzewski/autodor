using Autodor.Modules.Invoicing.Features.CreateInvoices;

namespace Autodor.Modules.Invoicing.Features.SendInvoiceSummaryEmail;

public record SendInvoiceSummaryEmailCommand(
    DateOnly DateFrom,
    DateOnly DateTo,
    CreateInvoicesResult InvoiceResult
);
