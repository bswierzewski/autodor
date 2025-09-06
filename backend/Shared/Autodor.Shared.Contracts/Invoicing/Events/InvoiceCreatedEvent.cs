using MediatR;

namespace Autodor.Shared.Contracts.Invoicing.Events;

public record InvoiceCreatedEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    IEnumerable<string> OrderNumbers,
    DateTime CreatedDate
) : INotification;