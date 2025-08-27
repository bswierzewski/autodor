using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

public record CreateBulkInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo,
    Guid ContractorId
) : IRequest<IEnumerable<Guid>>;