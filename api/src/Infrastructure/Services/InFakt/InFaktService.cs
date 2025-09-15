using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Entities;
using Infrastructure.Services.InFakt.DTOs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services.InFakt;

public class InFaktService : IInvoiceService
{
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;

    public InFaktService(IOptions<InFaktOptions> config, HttpClient httpClient)
    {
        _options = config.Value;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-inFakt-ApiKey", _options.ApiKey);
    }

    public async Task<Result<string>> AddInvoice(Invoice invoice)
    {
        try
        {
            // First create or update the contractor
            var contractorResult = await CreateOrUpdateContractor(invoice.Contractor);
            if (!contractorResult.IsSuccess)
            {
                return Result<string>.Failure($"Błąd tworzenia/aktualizacji kontrahenta: {contractorResult.Error}");
            }

            var invoiceDto = MapToInFaktFormat(invoice);
            var json = JsonSerializer.Serialize(invoiceDto);

            var url = $"{_options.ApiUrl}/async/invoices.json";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var statusResponse = JsonSerializer.Deserialize<InFaktStatusResponseDto>(responseContent);

                if (statusResponse?.InvoiceTaskReferenceNumber != null)
                {
                    await Task.Delay(5000);

                    var finalStatus = await CheckInvoiceStatus(statusResponse.InvoiceTaskReferenceNumber);
                    return finalStatus;
                }

                return Result<string>.Success(invoice.Contractor.NIP);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var parsedError = ParseInFaktError(errorContent);
                return Result<string>.Failure($"{parsedError}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd: {ex.Message}");
        }
    }

    private async Task<InFaktContractorResponseDto> FindContractorByNIP(string nip)
    {
        try
        {
            var url = $"{_options.ApiUrl}/clients.json?q[clean_nip_eq]={nip}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
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

    private async Task<Result<string>> UpdateContractor(int contractorId, Contractor contractor)
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

            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success("Kontrahent zaktualizowany");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var parsedError = ParseInFaktError(errorContent);
                return Result<string>.Failure($"Błąd aktualizacji kontrahenta: {parsedError}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd aktualizacji kontrahenta: {ex.Message}");
        }
    }

    private async Task<Result<string>> CreateOrUpdateContractor(Contractor contractor)
    {
        try
        {
            // First check if contractor already exists
            var existingContractor = await FindContractorByNIP(contractor.NIP);

            if (existingContractor != null)
            {
                // Update existing contractor
                return await UpdateContractor(existingContractor.Id, contractor);
            }
            else
            {
                // Create new contractor
                return await CreateNewContractor(contractor);
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd tworzenia/aktualizacji kontrahenta: {ex.Message}");
        }
    }

    private async Task<Result<string>> CreateNewContractor(Contractor contractor)
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

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return Result<string>.Success("Kontrahent utworzony");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var parsedError = ParseInFaktError(errorContent);
                return Result<string>.Failure($"{parsedError}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd tworzenia kontrahenta: {ex.Message}");
        }
    }

    private async Task<Result<string>> CheckInvoiceStatus(string taskReferenceNumber)
    {
        try
        {
            var url = $"{_options.ApiUrl}/async/invoices/status/{taskReferenceNumber}.json";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var statusResponse = JsonSerializer.Deserialize<InFaktStatusResponseDto>(responseContent);

                if (statusResponse.ProcessingCode == 201)
                {
                    return Result<string>.Success($"Faktura wystawiona");
                }
                else if (statusResponse.ProcessingCode == 422)
                {
                    var errorMessage = statusResponse.ProcessingDescription;
                    if (statusResponse.InvoiceErrors?.Base != null && statusResponse.InvoiceErrors.Base.Length > 0)
                    {
                        errorMessage += $": {string.Join("; ", statusResponse.InvoiceErrors.Base)}";
                    }
                    return Result<string>.Failure($"{errorMessage}");
                }
                else
                {
                    return Result<string>.Failure($"Status przetwarzania: {statusResponse.ProcessingDescription} (kod: {statusResponse.ProcessingCode})");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var parsedError = ParseInFaktError(errorContent);
                return Result<string>.Failure($"Błąd sprawdzania statusu: {parsedError}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd sprawdzania statusu: {ex.Message}");
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