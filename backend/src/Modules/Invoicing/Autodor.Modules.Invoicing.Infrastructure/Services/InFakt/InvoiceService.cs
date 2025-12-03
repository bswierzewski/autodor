using System.Net.Http.Json;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy _retryPolicy;

    public InvoiceService(
        ILogger<InvoiceService> logger,
        IOptions<InFaktOptions> config,
        HttpClient httpClient)
    {
        _logger = logger;
        _options = config.Value;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-inFakt-ApiKey", _options.ApiKey);

        _retryPolicy = Policy
            .Handle<TaskInProgressException>()
            .WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(2));
    }

    public async Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating invoice for contractor {ContractorName} with NIP {NIP}",
            invoice.Contractor.Name, invoice.Contractor.NIP);

        var invoiceRequest = invoice.ToInvoiceDto();
        var response = await _httpClient.PostAsJsonAsync($"{_options.ApiUrl}/async/invoices.json", invoiceRequest, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create invoice. Status: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
            throw new InvalidOperationException($"Failed to create invoice: {responseContent}");
        }

        var initialResponse = JsonSerializer.Deserialize<StatusResponseDto>(responseContent);
        if (string.IsNullOrEmpty(initialResponse?.InvoiceTaskReferenceNumber))
            throw new InvalidOperationException("Invoice creation failed - no reference number returned");

        _logger.LogInformation("Invoice creation started with reference number {ReferenceNumber}", initialResponse.InvoiceTaskReferenceNumber);

        string? invoiceNumber = null;
        await _retryPolicy.ExecuteAsync(async () =>
        {
            invoiceNumber = await CheckInvoiceStatusAsync(initialResponse.InvoiceTaskReferenceNumber, invoice.Contractor.NIP, cancellationToken);
        });

        if (invoiceNumber == null)
            throw new InvalidOperationException("Invoice creation failed - no invoice number returned after retries");

        _logger.LogInformation("Invoice created successfully for contractor {ContractorName} with number {InvoiceNumber}",
            invoice.Contractor.Name, invoiceNumber);

        return invoiceNumber;
    }

    private async Task<string?> CheckInvoiceStatusAsync(string taskReferenceNumber, string nip, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking invoice status for reference {ReferenceNumber}, NIP {NIP}", taskReferenceNumber, nip);

        var statusResponse = await _httpClient.GetAsync($"{_options.ApiUrl}/async/invoices/status/{taskReferenceNumber}.json", cancellationToken);
        var responseContent = await statusResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!statusResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Error checking invoice status for NIP {NIP}. Status: {StatusCode}, Content: {Content}",
                nip, statusResponse.StatusCode, responseContent);
            throw new InvalidOperationException($"Failed to check invoice status: {responseContent}");
        }

        var status = JsonSerializer.Deserialize<StatusResponseDto>(responseContent)
            ?? throw new InvalidOperationException("Failed to deserialize status response");

        _logger.LogDebug("Invoice status for NIP {NIP}: ProcessingCode={ProcessingCode}, Description={Description}",
            nip, status.ProcessingCode, status.ProcessingDescription);

        return status.ProcessingCode switch
        {
            201 => status.Invoice?.Number ?? $"INFAKT-{status.InvoiceTaskReferenceNumber}",
            422 => throw new InvalidOperationException($"Invoice creation failed for NIP {nip}: {status.ProcessingDescription}"),
            100 or 120 or 140 => throw new TaskInProgressException(status.ProcessingCode, status.ProcessingDescription ?? "Task in progress"),
            _ => throw new InvalidOperationException($"Unknown processing status for NIP {nip}: {status.ProcessingDescription} (code: {status.ProcessingCode})")
        };
    }
}