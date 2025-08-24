namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IInvoiceService
{
    Task<bool> SendInvoiceAsync(Guid invoiceId, string recipientEmail, CancellationToken cancellationToken = default);
    Task<string> GetInvoiceStatusAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}