using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

public record CreateInvoiceCommand(
    Guid ContractorId,
    IEnumerable<string> OrderNumbers
) : IRequest<Guid>;