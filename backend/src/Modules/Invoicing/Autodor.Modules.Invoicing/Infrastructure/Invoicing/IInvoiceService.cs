using Autodor.Modules.Invoicing.Domain.Aggregates;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing;

public interface IInvoiceService
{
    Task<ErrorOr<Success>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}