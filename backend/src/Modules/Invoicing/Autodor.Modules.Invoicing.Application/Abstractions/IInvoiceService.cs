using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoiceService
{
    /// <summary>
    /// Creates an invoice in the external invoicing system.
    /// Throws exceptions for technical errors (network, API errors, etc.)
    /// </summary>
    /// <returns>True if invoice was created successfully</returns>
    /// <exception cref="InvalidOperationException">When the external API returns an error</exception>
    /// <exception cref="HttpRequestException">When network communication fails</exception>
    Task<bool> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
}