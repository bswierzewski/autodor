using MediatR;

namespace Autodor.Shared.Contracts.Invoicing.Commands;

public record CreateBulkInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo,
    Guid ContractorId
) : IRequest<IEnumerable<Guid>>;