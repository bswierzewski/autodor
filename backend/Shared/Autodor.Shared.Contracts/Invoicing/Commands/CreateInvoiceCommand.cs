using MediatR;

namespace Autodor.Shared.Contracts.Invoicing.Commands;

public record CreateInvoiceCommand(
    Guid ContractorId,
    IEnumerable<string> OrderNumbers
) : IRequest<Guid>;