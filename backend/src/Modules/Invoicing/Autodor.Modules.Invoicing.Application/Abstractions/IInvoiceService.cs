using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoiceService
{
    Task<bool> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}