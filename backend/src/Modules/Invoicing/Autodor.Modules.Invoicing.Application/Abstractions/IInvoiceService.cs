using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoiceServiceFactory
{
    /// <summary>
    /// Gets the appropriate invoice service implementation
    /// </summary>
    IInvoiceService GetInvoiceService();
}

public interface IInvoiceService
{
    /// <summary>
    /// Creates an invoice in the external invoicing system
    /// </summary>
    Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}