namespace Autodor.Modules.Invoicing.Application.Abstractions;

public interface IPdfGeneratorService
{
    Task<byte[]> GenerateInvoicePdfAsync(object invoiceData, CancellationToken cancellationToken = default);
}