using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class InFaktContractorService
{
    private readonly ILogger<InFaktContractorService> _logger;
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;

    public InFaktContractorService(
        ILogger<InFaktContractorService> logger,
        IOptions<InFaktOptions> config,
        HttpClient httpClient)
    {
        _logger = logger;
        _options = config.Value;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-inFakt-ApiKey", _options.ApiKey);
    }

    public async Task<InFaktContractorResponseDto> FindContractorByNIPAsync(string nip, CancellationToken cancellationToken = default)
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

    public async Task<bool> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
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

    public async Task<bool> UpdateContractorAsync(int contractorId, Contractor contractor, CancellationToken cancellationToken = default)
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