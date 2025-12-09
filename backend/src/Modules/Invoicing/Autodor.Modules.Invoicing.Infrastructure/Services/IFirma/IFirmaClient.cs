using System.Text;
using System.Text.Json;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Infrastructure.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;

public class IFirmaClient
{
    private readonly IFirmaOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IDictionary<string, string> _apiKeys;

    public IFirmaClient(IOptions<IFirmaOptions> config, HttpClient httpClient)
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

    public async Task<ResponseDto> CreateInvoiceAsync(InvoiceDto invoiceDto, CancellationToken cancellationToken = default)
    {
        string keyName = "faktura";
        string key = _apiKeys[keyName];

        var response = await PostAsync("https://www.ifirma.pl/iapi/fakturakraj.json", keyName, key, invoiceDto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to create invoice. HTTP {response.StatusCode}: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var iFirmaResponse = JsonSerializer.Deserialize<ResponseDto>(responseContent);

        if (iFirmaResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize IFirma response");
        }

        return iFirmaResponse;
    }

    private async Task<HttpResponseMessage> PostAsync(string url, string keyName, string key, object content, CancellationToken cancellationToken)
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
