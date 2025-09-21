using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class IFirmaInvoiceService : IInvoiceService
{
    private readonly ILogger<IFirmaInvoiceService> _logger;
    private readonly IFirmaOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IDictionary<string, string> _apiKeys;

    public IFirmaInvoiceService(
        ILogger<IFirmaInvoiceService> logger,
        IOptions<IFirmaOptions> config,
        HttpClient httpClient)
    {
        _logger = logger;
        _options = config.Value;
        _httpClient = httpClient;
        _apiKeys = new Dictionary<string, string>()
        {
            {"faktura", _options.Faktura },
            {"abonent", _options.Abonent },
            {"rachunek", _options.Rachunek },
            {"wydatek", _options.Wydatek },
        };
    }

    public async Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            string keyName = "faktura";
            string key = _apiKeys[keyName];

            var invoiceDto = MapToInvoiceDto(invoice);
            var response = await Post("https://www.ifirma.pl/iapi/fakturakraj.json", keyName, key, invoiceDto, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var iFirmaResponse = JsonSerializer.Deserialize<IFirmaResponseDto>(responseContent);

                if (iFirmaResponse?.Response?.Kod == 0 && !string.IsNullOrEmpty(iFirmaResponse.Response.Identyfikator))
                {
                    var invoiceId = iFirmaResponse.Response.Identyfikator;
                    _logger.LogInformation("Invoice created successfully for contractor: {ContractorName} with ID: {InvoiceId}", invoice.Contractor.Name, invoiceId);
                    return invoiceId; // Return the invoice ID from IFirma
                }
                else
                {
                    var errorMsg = iFirmaResponse?.Response?.Informacja ?? "Unknown error";
                    throw new InvalidOperationException($"Failed to create invoice: {errorMsg}");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to create invoice. HTTP {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to create invoice. HTTP {response.StatusCode}: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for contractor: {ContractorName}", invoice.Contractor.Name);
            throw;
        }
    }

    private IFirmaInvoiceDto MapToInvoiceDto(Invoice invoice)
    {
        return new IFirmaInvoiceDto
        {
            Numer = invoice.Number,
            DataWystawienia = invoice.IssueDate.ToString("yyyy-MM-dd"),
            DataSprzedazy = invoice.SaleDate.ToString("yyyy-MM-dd"),
            MiejsceWystawienia = invoice.PlaceOfIssue,
            TerminPlatnosci = invoice.PaymentDue,
            SposobZaplaty = invoice.PaymentMethod,
            Uwagi = invoice.Notes,
            Kontrahent = new IFirmaKontrahent
            {
                Nazwa = invoice.Contractor.Name,
                NIP = invoice.Contractor.NIP,
                Ulica = invoice.Contractor.Street,
                KodPocztowy = invoice.Contractor.ZipCode,
                Miejscowosc = invoice.Contractor.City,
                Kraj = "PL", // Default to Poland
                Email = invoice.Contractor.Email,
                Telefon = "" // Not available in current Contractor model
            },
            Pozycje = invoice.Items.Select(item => new IFirmaPozycje
            {
                NazwaPelna = item.Name,
                Jednostka = item.Unit,
                Ilosc = item.Quantity,
                CenaJednostkowa = (float)item.UnitPrice,
                StawkaVat = item.VatRate,
                TypStawkiVat = item.VatType,
                Rabat = (int)item.Discount,
                PKWiU = item.PKWiU,
                GTU = item.GTU
            }).ToArray()
        };
    }

    private async Task<HttpResponseMessage> Post(string url, string keyName, string key, object content, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(content);
        SetAuthenticationHeader(url, keyName, key, json);

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(url, httpContent, cancellationToken);
    }

    private void SetAuthenticationHeader(string url, string keyName, string key, string content = "")
    {
        _httpClient.DefaultRequestHeaders.Clear();

        url = url.Split('?')[0];
        string hmac = $"{url}{_options.User}{keyName}{content}";
        string sha1 = hmac.HmacSHA1(key);

        _httpClient.DefaultRequestHeaders.Add("Authentication", $"IAPIS user={_options.User}, hmac-sha1={sha1}");
    }
}