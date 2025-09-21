using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class InFaktInvoiceService : IInvoiceService
{
    private readonly ILogger<InFaktInvoiceService> _logger;
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;
    private static readonly int[] _retryDelays = { 1000, 2000, 4000, 8000, 15000, 30000 }; // milliseconds

    public InFaktInvoiceService(
        ILogger<InFaktInvoiceService> logger,
        IOptions<InFaktOptions> config,
        HttpClient httpClient)
    {
        _logger = logger;
        _options = config.Value;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-inFakt-ApiKey", _options.ApiKey);
    }

    public async Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            var invoiceDto = MapToInFaktFormat(invoice);
            var json = JsonSerializer.Serialize(invoiceDto);

            var url = $"{_options.ApiUrl}/async/invoices.json";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var statusResponse = JsonSerializer.Deserialize<InFaktStatusResponseDto>(responseContent);

                if (statusResponse?.InvoiceTaskReferenceNumber != null)
                {
                    var invoiceNumber = await PollInvoiceStatusAsync(statusResponse.InvoiceTaskReferenceNumber, invoice.Contractor.NIP, cancellationToken);
                    if (!string.IsNullOrEmpty(invoiceNumber))
                    {
                        _logger.LogInformation("Invoice created successfully for contractor: {ContractorName} with number: {InvoiceNumber}", invoice.Contractor.Name, invoiceNumber);
                        return invoiceNumber;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invoice creation failed during status check");
                    }
                }

                _logger.LogInformation("Invoice created successfully for contractor: {ContractorName}", invoice.Contractor.Name);
                return $"INFAKT-{statusResponse.InvoiceTaskReferenceNumber}"; // Return task reference as fallback
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ParseInFaktError(errorContent);
                _logger.LogError("Failed to create invoice. Error: {Error}", parsedError);
                throw new InvalidOperationException($"Failed to create invoice: {parsedError}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for contractor: {ContractorName}", invoice.Contractor.Name);
            throw;
        }
    }


    private async Task<string?> PollInvoiceStatusAsync(string taskReferenceNumber, string nip, CancellationToken cancellationToken = default)
    {
        for (int attempt = 0; attempt < _retryDelays.Length; attempt++)
        {
            try
            {
                var invoiceNumber = await CheckInvoiceStatusAsync(taskReferenceNumber, nip, cancellationToken);
                return invoiceNumber; // Success - invoice created
            }
            catch (TaskInProgressException ex)
            {
                _logger.LogInformation("Invoice still in progress (attempt {Attempt}/{MaxAttempts}). Code: {Code}, Description: {Description}",
                    attempt + 1, _retryDelays.Length, ex.ProcessingCode, ex.ProcessingDescription);

                if (attempt < _retryDelays.Length - 1) // Not the last attempt
                {
                    await Task.Delay(_retryDelays[attempt], cancellationToken);
                }
                else
                {
                    _logger.LogError("Invoice polling timeout after {MaxAttempts} attempts for NIP: {NIP}", _retryDelays.Length, nip);
                    return null; // Timeout
                }
            }
            catch (Exception)
            {
                // Permanent failure
                return null;
            }
        }

        return null; // Should never reach here
    }

    private async Task<string?> CheckInvoiceStatusAsync(string taskReferenceNumber, string nip, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_options.ApiUrl}/async/invoices/status/{taskReferenceNumber}.json";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var statusResponse = JsonSerializer.Deserialize<InFaktStatusResponseDto>(responseContent);

                return statusResponse.ProcessingCode switch
                {
                    201 => // Faktura stworzona - SUCCESS
                        statusResponse.Invoice?.Number ?? $"INFAKT-{statusResponse.InvoiceTaskReferenceNumber}",
                    422 => // Nie udało się stworzyć faktury - PERMANENT FAILURE
                        throw new InvalidOperationException(GetErrorMessage(statusResponse, nip)),
                    100 or 120 or 140 => // Zlecenie przyjęte/czeka/w trakcie - CONTINUE POLLING
                        throw new TaskInProgressException(statusResponse.ProcessingCode, statusResponse.ProcessingDescription ?? "Task in progress"),
                    _ => // Unknown status - FAILURE
                        throw new InvalidOperationException($"Unknown processing status for NIP {nip}: {statusResponse.ProcessingDescription} (code: {statusResponse.ProcessingCode})")
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ParseInFaktError(errorContent);
                _logger.LogError("Error checking invoice status for NIP {NIP}: {Error}", nip, parsedError);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking invoice status for NIP: {NIP}", nip);
            return null;
        }
    }

    private InFaktInvoiceRequestDto MapToInFaktFormat(Invoice invoice)
    {
        return new InFaktInvoiceRequestDto
        {
            Invoice = new InFaktInvoiceDto
            {
                PaymentMethod = "transfer",
                BankName = "PKO BANK POLSKI",
                BankAccount = "56102030880000820200920322",
                PaymentDate = invoice.PaymentDue.ToString("yyyy-MM-dd"),
                ClientTaxCode = invoice.Contractor.NIP,
                Services = invoice.Items.Select(item => new InFaktServiceDto
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    NetPrice = (int)(item.UnitPrice * item.Quantity * 100), // konwersja na grosze
                    TaxSymbol = (int)(item.VatRate * 100) // konwersja np. 0.23 -> 23
                }).ToArray()
            }
        };
    }

    private string ParseInFaktError(string errorContent)
    {
        try
        {
            var errorDto = JsonSerializer.Deserialize<InFaktErrorDto>(errorContent);

            if (errorDto?.Errors == null && string.IsNullOrEmpty(errorDto?.Message))
                return errorContent;

            var errors = new List<string>();

            if (!string.IsNullOrEmpty(errorDto.Message))
                errors.Add(errorDto.Message);

            if (errorDto.Errors != null)
            {
                foreach (var error in errorDto.Errors)
                {
                    errors.AddRange(error.Value.Where(e => !string.IsNullOrEmpty(e)));
                }
            }

            return errors.Count > 0 ? string.Join("; ", errors) : errorContent;
        }
        catch
        {
            return errorContent;
        }
    }

    private string GetErrorMessage(InFaktStatusResponseDto statusResponse, string nip)
    {
        var errorMessage = statusResponse.ProcessingDescription ?? "Unknown error";
        if (statusResponse.InvoiceErrors?.Base != null && statusResponse.InvoiceErrors.Base.Length > 0)
        {
            errorMessage += $": {string.Join("; ", statusResponse.InvoiceErrors.Base)}";
        }
        _logger.LogError("Invoice creation failed for NIP {NIP}: {Error}", nip, errorMessage);
        return $"Invoice creation failed for NIP {nip}: {errorMessage}";
    }
}