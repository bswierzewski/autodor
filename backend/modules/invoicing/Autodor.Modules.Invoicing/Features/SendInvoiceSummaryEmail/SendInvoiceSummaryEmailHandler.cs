using Autodor.Modules.Invoicing.Features.CreateInvoices;
using Autodor.Modules.Invoicing.Infrastructure.Email;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Features.SendInvoiceSummaryEmail;

public static class SendInvoiceSummaryEmailHandler
{
    public static async Task Handle(
        SendInvoiceSummaryEmailCommand command,
        IEmailSender emailSender,
        IOptions<InvoicingOptions> options,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(typeof(SendInvoiceSummaryEmailHandler));
        var recipients = options.Value.Scheduling.SummaryRecipients;
        if (recipients.Count == 0)
        {
            logger.LogDebug("No summary recipients configured, skipping invoice summary email");
            return;
        }

        var subject = $"Faktury zbiorcze {command.DateFrom:dd.MM.yyyy}–{command.DateTo:dd.MM.yyyy}: " +
                      $"utworzono {command.InvoiceResult.InvoicesCreated}";

        await emailSender.SendAsync(
            new EmailMessage(recipients, subject, BuildEmailBody(command)),
            ct);

        logger.LogInformation("Invoice summary email sent for range {DateFrom}–{DateTo}",
            command.DateFrom, command.DateTo);
    }

    private static string BuildEmailBody(SendInvoiceSummaryEmailCommand command)
    {
        var (dateFrom, dateTo, result) = command;

        var rows = string.Join("\n", result.Details.Select(d =>
        {
            var statusColor = d.Success ? "#2e7d32" : "#c62828";
            var statusText = d.Success ? "✓ Utworzono" : $"✗ {System.Net.WebUtility.HtmlEncode(d.ErrorMessage)}";
            return $"""
                <tr>
                  <td style="padding:6px 10px;border-bottom:1px solid #eee">{System.Net.WebUtility.HtmlEncode(d.ContractorName)}</td>
                  <td style="padding:6px 10px;border-bottom:1px solid #eee;font-family:monospace">{System.Net.WebUtility.HtmlEncode(d.ContractorNip)}</td>
                  <td style="padding:6px 10px;border-bottom:1px solid #eee;text-align:center">{d.ItemCount}</td>
                  <td style="padding:6px 10px;border-bottom:1px solid #eee;color:{statusColor}">{statusText}</td>
                </tr>
                """;
        }));

        return $"""
            <html><body style="font-family:Arial,sans-serif;color:#333;max-width:800px">
              <h2>Zbiorcze tworzenie faktur</h2>
              <p>Zakres dat: <strong>{dateFrom:dd.MM.yyyy} – {dateTo:dd.MM.yyyy}</strong></p>
              <table style="border-collapse:collapse;width:100%;margin-top:16px">
                <thead>
                  <tr style="background:#f5f5f5">
                    <th style="padding:8px 10px;text-align:left;border-bottom:2px solid #ddd">Kontrahent</th>
                    <th style="padding:8px 10px;text-align:left;border-bottom:2px solid #ddd">NIP</th>
                    <th style="padding:8px 10px;text-align:center;border-bottom:2px solid #ddd">Pozycji</th>
                    <th style="padding:8px 10px;text-align:left;border-bottom:2px solid #ddd">Status</th>
                  </tr>
                </thead>
                <tbody>
            {rows}
                </tbody>
              </table>
              <p style="margin-top:16px">
                Łącznie: <strong>{result.InvoicesCreated} faktur utworzono</strong>,
                {result.InvoicesSkipped} pominięto.
              </p>
            </body></html>
            """;
    }
}
