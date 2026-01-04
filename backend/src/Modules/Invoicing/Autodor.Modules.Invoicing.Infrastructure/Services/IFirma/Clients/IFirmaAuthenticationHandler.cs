using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private readonly IFirmaOptions _options = options.Value
        ?? throw new ArgumentNullException(nameof(options));

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Retrieve the API key type from the request via extension method
        var keyType = request.GetApiKey();

        // 2. If the request has a valid URI and an API key type was specified:
        if (request.RequestUri is not null && keyType.HasValue)
        {
            // 3. Resolve the enum to the actual key name and secret value from configuration
            var (keyName, keyHex) = ResolveKeyDetails(keyType.Value);

            string content = await GetContentAsync(request, cancellationToken);

            // 4. Compute the HMAC signature using the full URI
            string signature = ComputeSignature(keyName, keyHex, request.RequestUri, content);

            request.Headers.Add(
                "Authentication",
                $"IAPIS user={_options.User}, hmac-sha1={signature}"
            );
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private (string KeyName, string KeyHex) ResolveKeyDetails(IFirmaKeyType type)
    {
        return type switch
        {
            IFirmaKeyType.Invoice => ("faktura", EnsureKey(_options.ApiKeys.Faktura, type)),

            IFirmaKeyType.Subscriber => ("abonent", EnsureKey(_options.ApiKeys.Abonent, type)),

            IFirmaKeyType.Account => ("rachunek", EnsureKey(_options.ApiKeys.Rachunek, type)),

            IFirmaKeyType.Expense => ("wydatek", EnsureKey(_options.ApiKeys.Wydatek, type)),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type,
                "Unsupported iFirma key type. Ensure the enum value is mapped in ResolveKeyDetails.")
        };
    }

    private static string EnsureKey(string? key, IFirmaKeyType type)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                $"API Key for '{type}' is missing or empty in configuration. " +
                $"Ensure IFirmaOptions.ApiKeys.{type} is properly configured.");
        }

        return key;
    }

    private string ComputeSignature(string keyName, string keyHex, Uri requestUri, string content)
    {
        // Get the full URL up to and including the path (e.g., https://www.ifirma.pl/iapi/fakturakraj)
        // This includes the scheme, authority, and path, but excludes query strings and fragments.
        string url = requestUri.GetLeftPart(UriPartial.Path);
        string user = _options.User ?? string.Empty;

        // Construct the message in the format expected by iFirma: URL + User + KeyName + Content
        string message = $"{url}{user}{keyName}{content}";

        return HmacSha1.Compute(keyHex, message);
    }

    private static Task<string> GetContentAsync(HttpRequestMessage request, CancellationToken ct) =>
        request.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(string.Empty);
}
