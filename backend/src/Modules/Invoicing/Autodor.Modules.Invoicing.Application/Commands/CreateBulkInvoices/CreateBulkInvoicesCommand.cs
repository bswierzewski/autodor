using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

public record CreateBulkInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo
) : IRequest<Dictionary<string, bool>>;