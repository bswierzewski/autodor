using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoicePreProcessor
{
    Task PrepareInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}