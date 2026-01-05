using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoices;

public record CreateInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo
) : IRequest<Dictionary<string, bool>>;