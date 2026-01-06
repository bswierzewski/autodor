using Autodor.Modules.Invoicing.Domain.ValueObjects;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoiceService
{
    Task<ErrorOr<bool>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}