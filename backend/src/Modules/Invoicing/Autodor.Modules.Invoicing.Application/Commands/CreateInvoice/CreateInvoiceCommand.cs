using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

public record CreateInvoiceCommand(
    int? InvoiceNumber,
    DateTime SaleDate,
    DateTime IssueDate,
    IEnumerable<DateTime> Dates,
    IEnumerable<string> OrderIds,
    Guid ContractorId
) : IRequest<Unit>;