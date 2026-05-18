using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private readonly IFirmaOptions _options = options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is { } uri)
        {
            var (keyName, keyHex) = ResolveKeyDetails(uri);
            var content = request.Content is not null
                ? await request.Content.ReadAsStringAsync(cancellationToken)
                : string.Empty;

            var message = $"{uri.GetLeftPart(UriPartial.Path)}{_options.User}{keyName}{content}";
            var signature = HmacSha1.Compute(keyHex, message);

            request.Headers.Add("Authentication", $"IAPIS user={_options.User}, hmac-sha1={signature}");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private (string KeyName, string KeyHex) ResolveKeyDetails(Uri uri)
    {
        var path = uri.AbsolutePath.ToLowerInvariant();

        return path switch
        {
            _ when path.Contains("faktura") => ("faktura", GetKey(_options.ApiKeys.Faktura)),
            _ when path.Contains("abonent") => ("abonent", GetKey(_options.ApiKeys.Abonent)),
            _ when path.Contains("rachunek") => ("rachunek", GetKey(_options.ApiKeys.Rachunek)),
            _ when path.Contains("wydatek") => ("wydatek", GetKey(_options.ApiKeys.Wydatek)),
            _ => throw new InvalidOperationException($"Cannot determine API key for path: {path}")
        };
    }

    private static string GetKey(string? key) =>
        string.IsNullOrWhiteSpace(key)
            ? throw new InvalidOperationException("API Key is missing in configuration.")
            : key;
}
