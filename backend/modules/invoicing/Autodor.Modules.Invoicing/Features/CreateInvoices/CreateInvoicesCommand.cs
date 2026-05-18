namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public record CreateInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo
);
