using Application.Common.Interfaces;
using Application.Common.Options;
using Application.Invoices.Commands.DTOs;
using Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class FirmaService : IFirmaService
{
    private readonly IFirmaOptions _options;

    private readonly IDictionary<string, string> _apiKeys;

    public FirmaService(IOptions<IFirmaOptions> config)
    {
        _options = config.Value;
        _apiKeys = new Dictionary<string, string>()
            {
                {"faktura", _options.Faktura },
                {"abonent", _options.Abonent },
                {"rachunek", _options.Rachunek },
                {"wydatek", _options.Wydatek },
            };
    }

    public async Task<HttpResponseMessage> AddInvoice(InvoiceDto invoice)
    {
        string keyName = "faktura";
        string key = _apiKeys[keyName];

        return await Post("https://www.ifirma.pl/iapi/fakturakraj.json", keyName, key, invoice);
    }

    private async Task<HttpResponseMessage> Get(string url, string keyName, string key)
    {
        using (var client = CreateClient(url, keyName, key))
        {
            return await client.GetAsync(url);
        }
    }

    private async Task<HttpResponseMessage> Post(string url, string keyName, string key, object content)
    {
        var json = JsonSerializer.Serialize(content);

        using (var client = CreateClient(url, keyName, key, json))
        {
            return await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }

    private HttpClient CreateClient(string url, string keyName, string key, string content = "")
    {
        var client = new HttpClient();

        url = url.Split('?')[0];

        string hmac = $"{url}{_options.User}{keyName}{content}";

        string sha1 = hmac.HmacSHA1(key);

        client.DefaultRequestHeaders.Add("Authentication", $"IAPIS user={_options.User}, hmac-sha1={sha1}");

        return client;
    }
}
