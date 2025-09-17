using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class InFaktInvoiceService : IInvoiceService
{
    private readonly ILogger<InFaktInvoiceService> _logger;
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;

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

    public async Task<Guid> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            // First create or update the contractor
            var contractorSuccess = await CreateOrUpdateContractor(invoice.Contractor, cancellationToken);
            if (!contractorSuccess)
            {
                throw new InvalidOperationException("Failed to create or update contractor");
            }

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
                    await Task.Delay(5000, cancellationToken);

                    var finalStatus = await CheckInvoiceStatus(statusResponse.InvoiceTaskReferenceNumber, invoice.Contractor.NIP, cancellationToken);
                    if (finalStatus)
                    {
                        _logger.LogInformation("Invoice created successfully for contractor: {ContractorName}", invoice.Contractor.Name);
                        return Guid.NewGuid();
                    }
                    else
                    {
                        throw new InvalidOperationException("Invoice creation failed during status check");
                    }
                }

                _logger.LogInformation("Invoice created successfully for contractor: {ContractorName}", invoice.Contractor.Name);
                return Guid.NewGuid();
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

    private async Task<InFaktContractorResponseDto> FindContractorByNIP(string nip, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_options.ApiUrl}/clients.json?q[clean_nip_eq]={nip}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var contractorList = JsonSerializer.Deserialize<InFaktContractorListResponseDto>(responseContent);

                return contractorList?.Entities?.FirstOrDefault();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> UpdateContractor(int contractorId, Contractor contractor, CancellationToken cancellationToken)
    {
        try
        {
            var contractorDto = new InFaktContractorRequestDto
            {
                Client = new InFaktContractorDto
                {
                    CompanyName = contractor.Name,
                    Street = contractor.Street,
                    City = contractor.City,
                    PostalCode = contractor.ZipCode,
                    NIP = contractor.NIP,
                    Country = "PL",
                    Email = contractor.Email
                }
            };

            var json = JsonSerializer.Serialize(contractorDto);
            var url = $"{_options.ApiUrl}/clients/{contractorId}.json";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ParseInFaktError(errorContent);
                _logger.LogError("Failed to update contractor: {Error}", parsedError);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contractor: {ContractorName}", contractor.Name);
            return false;
        }
    }

    private async Task<bool> CreateOrUpdateContractor(Contractor contractor, CancellationToken cancellationToken)
    {
        try
        {
            // First check if contractor already exists
            var existingContractor = await FindContractorByNIP(contractor.NIP, cancellationToken);

            if (existingContractor != null)
            {
                // Update existing contractor
                return await UpdateContractor(existingContractor.Id, contractor, cancellationToken);
            }
            else
            {
                // Create new contractor
                return await CreateNewContractor(contractor, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or updating contractor: {ContractorName}", contractor.Name);
            return false;
        }
    }

    private async Task<bool> CreateNewContractor(Contractor contractor, CancellationToken cancellationToken)
    {
        try
        {
            var contractorDto = new InFaktContractorRequestDto
            {
                Client = new InFaktContractorDto
                {
                    CompanyName = contractor.Name,
                    Street = contractor.Street,
                    City = contractor.City,
                    PostalCode = contractor.ZipCode,
                    NIP = contractor.NIP,
                    Country = "PL",
                    Email = contractor.Email
                }
            };

            var json = JsonSerializer.Serialize(contractorDto);
            var url = $"{_options.ApiUrl}/clients.json";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ParseInFaktError(errorContent);
                _logger.LogError("Failed to create contractor: {Error}", parsedError);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contractor: {ContractorName}", contractor.Name);
            return false;
        }
    }

    private async Task<bool> CheckInvoiceStatus(string taskReferenceNumber, string nip, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_options.ApiUrl}/async/invoices/status/{taskReferenceNumber}.json";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var statusResponse = JsonSerializer.Deserialize<InFaktStatusResponseDto>(responseContent);

                if (statusResponse.ProcessingCode == 201)
                {
                    _logger.LogInformation("Invoice created successfully for NIP: {NIP}", nip);
                    return true;
                }
                else if (statusResponse.ProcessingCode == 422)
                {
                    var errorMessage = statusResponse.ProcessingDescription;
                    if (statusResponse.InvoiceErrors?.Base != null && statusResponse.InvoiceErrors.Base.Length > 0)
                    {
                        errorMessage += $": {string.Join("; ", statusResponse.InvoiceErrors.Base)}";
                    }
                    _logger.LogError("Invoice creation failed for NIP {NIP}: {Error}", nip, errorMessage);
                    return false;
                }
                else
                {
                    _logger.LogError("Invoice processing status for NIP {NIP}: {Description} (code: {Code})", nip, statusResponse.ProcessingDescription, statusResponse.ProcessingCode);
                    return false;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ParseInFaktError(errorContent);
                _logger.LogError("Error checking invoice status for NIP {NIP}: {Error}", nip, parsedError);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking invoice status for NIP: {NIP}", nip);
            return false;
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
}