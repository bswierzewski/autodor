namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public record CreateInvoicesCommand(
    Guid[] OrderIds
    // TODO: Add other invoice creation parameters
);
