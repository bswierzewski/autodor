using Autodor.Modules.Invoicing.Domain.Aggregates;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing;

public interface IInvoiceService
{
    Task CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}