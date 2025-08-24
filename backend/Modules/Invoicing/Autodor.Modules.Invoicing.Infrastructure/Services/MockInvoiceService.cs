using Autodor.Modules.Invoicing.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class MockInvoiceService : IInvoiceService
{
    private readonly ILogger<MockInvoiceService> _logger;
    private static readonly Dictionary<Guid, string> _invoiceStatuses = new();

    public MockInvoiceService(ILogger<MockInvoiceService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendInvoiceAsync(Guid invoiceId, string recipientEmail, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending invoice {InvoiceId} to {RecipientEmail}", invoiceId, recipientEmail);

        // Symulacja wysyłania faktury
        await Task.Delay(200, cancellationToken);

        // Symulacja losowego powodzenia/niepowodzenia (90% szans na sukces)
        var random = new Random();
        var success = random.NextDouble() > 0.1;

        if (success)
        {
            _invoiceStatuses[invoiceId] = "Sent";
            _logger.LogInformation("Invoice {InvoiceId} sent successfully to {RecipientEmail}", invoiceId, recipientEmail);
        }
        else
        {
            _invoiceStatuses[invoiceId] = "Send Failed";
            _logger.LogWarning("Failed to send invoice {InvoiceId} to {RecipientEmail}", invoiceId, recipientEmail);
        }

        return success;
    }

    public async Task<string> GetInvoiceStatusAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting status for invoice {InvoiceId}", invoiceId);

        await Task.Delay(50, cancellationToken); // Symulacja opóźnienia

        var status = _invoiceStatuses.GetValueOrDefault(invoiceId, "Draft");
        
        _logger.LogInformation("Invoice {InvoiceId} status: {Status}", invoiceId, status);

        return status;
    }
}