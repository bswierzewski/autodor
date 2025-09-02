using System.Text;
using System.Text.Json;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Options;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Services.IFirma.DTOs;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class FirmaService : IInvoiceService
{
    private readonly IFirmaOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IDictionary<string, string> _apiKeys;

    public FirmaService(IOptions<IFirmaOptions> config, HttpClient httpClient)
    {
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

    public async Task<Result<string>> AddInvoice(Invoice invoice)
    {
        try
        {
            string keyName = "faktura";
            string key = _apiKeys[keyName];

            var invoiceDto = MapToInvoiceDto(invoice);

            var response = await Post("https://www.ifirma.pl/iapi/fakturakraj.json", keyName, key, invoiceDto);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<IFirmaResponseDto>(responseContent);

                if (apiResponse?.Response?.Kod == 0)
                {
                    return Result<string>.Success($"{invoice.Contractor.Name}");
                }
                else
                {
                    var errorMessage = apiResponse?.Response?.Informacja ?? "Nieznany błąd API";
                    return Result<string>.Failure($"{apiResponse?.Response?.Kod}: {errorMessage}");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure($"Błąd HTTP {response.StatusCode}: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Błąd: {ex.Message}");
        }
    }

    private IFirmaInvoiceDto MapToInvoiceDto(Invoice invoice)
    {
        return new IFirmaInvoiceDto
        {
            Numer = invoice.Number,
            DataWystawienia = invoice.IssueDate.ToString("yyyy-MM-dd"),
            DataSprzedazy = invoice.SaleDate.ToString("yyyy-MM-dd"),
            TerminPlatnosci = invoice.PaymentDue.ToString("yyyy-MM-dd"),
            MiejsceWystawienia = "Leszno",
            Zaplacono = 0,
            ZaplaconoNaDokumencie = 0,
            LiczOd = "BRT",
            FormatDatySprzedazy = "DZN",
            SposobZaplaty = "PRZ",
            RodzajPodpisuOdbiorcy = "OUP",
            NazwaSeriiNumeracji = "Domyślna roczna",
            Kontrahent = new IFirmaKontrahent
            {
                NIP = invoice.Contractor.NIP,
                Nazwa = invoice.Contractor.Name,
                Miejscowosc = invoice.Contractor.City,
                Ulica = invoice.Contractor.Street,
                KodPocztowy = invoice.Contractor.ZipCode,
                Email = invoice.Contractor.Email,
            },
            Pozycje = invoice.Items.Select(item => new IFirmaPozycje
            {
                Ilosc = item.Quantity,
                CenaJednostkowa = (float)Math.Round(item.UnitPrice * 1.23M, 2),
                Jednostka = item.Unit,
                NazwaPelna = item.Name,
                StawkaVat = item.VatRate,
                TypStawkiVat = item.VatType,
            }).ToArray()
        };
    }

    private async Task<HttpResponseMessage> Post(string url, string keyName, string key, object content)
    {
        var json = JsonSerializer.Serialize(content);
        SetAuthenticationHeader(url, keyName, key, json);

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(url, httpContent);
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
