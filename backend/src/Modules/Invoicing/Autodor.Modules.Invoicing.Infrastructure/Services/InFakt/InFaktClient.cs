using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class InFaktClient
{
    private readonly InFaktOptions _options;
    private readonly HttpClient _httpClient;

    public InFaktClient(IOptions<InFaktOptions> config, HttpClient httpClient)
    {
        _options = config.Value;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-inFakt-ApiKey", _options.ApiKey);
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(InvoiceRequestDto invoiceRequest, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_options.ApiUrl}/invoices.json", invoiceRequest, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Failed to create invoice. Status: {response.StatusCode}, Content: {responseContent}");

        var invoiceResponse = JsonSerializer.Deserialize<InvoiceResponseDto>(responseContent);
        if (invoiceResponse == null)
            throw new InvalidOperationException("Failed to deserialize invoice creation response");

        return invoiceResponse;
    }

    public async Task<ContractorResponseDto?> FindContractorByNIPAsync(string nip, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.ApiUrl}/clients.json?q[clean_nip_eq]={nip}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var contractorList = JsonSerializer.Deserialize<ContractorListResponseDto>(responseContent);

        return contractorList?.Entities?.FirstOrDefault();
    }

    public async Task<HttpResponseMessage> CreateContractorAsync(ContractorRequestDto contractorDto, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(contractorDto);
        var url = $"{_options.ApiUrl}/clients.json";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(url, content, cancellationToken);
    }

    public async Task<HttpResponseMessage> UpdateContractorAsync(int contractorId, ContractorRequestDto contractorDto, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(contractorDto);
        var url = $"{_options.ApiUrl}/clients/{contractorId}.json";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PutAsync(url, content, cancellationToken);
    }
}
