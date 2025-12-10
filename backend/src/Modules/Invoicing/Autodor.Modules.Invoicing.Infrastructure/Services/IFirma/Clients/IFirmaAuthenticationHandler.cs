using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;
using Autodor.Modules.Invoicing.Application.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

/// <summary>
/// A DelegatingHandler that automatically signs HTTP requests with iFirma specific HMAC-SHA1 headers.
/// It uses a routing map to determine which API Key to use based on the request URL.
/// </summary>
public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private readonly IFirmaOptions _config = options.Value;

    /// <summary>
    /// High-performance read-only dictionary for routing.
    /// Maps URL suffixes to specific configuration keys.
    /// </summary>
    private static readonly FrozenDictionary<string, Func<IFirmaOptions, string?>> EndpointMap =
        new Dictionary<string, Func<IFirmaOptions, string?>>
        {
            // Invoices (Faktura)
            { "fakturakraj.json", opt => opt.Keys.Faktura },
            { "fakturakraj/pobierz.json", opt => opt.Keys.Faktura },

            // Contractors (Abonent)
            { "abonent.json", opt => opt.Keys.Abonent },
            { "abonent/szukaj.json", opt => opt.Keys.Abonent },

            // Bills (Rachunek)
            { "rachunek.json", opt => opt.Keys.Rachunek },

            // CRM/Agenda
            { "agenda.json", opt => opt.Keys.Agenda }
        }.ToFrozenDictionary();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Determine which key to use based on the Request URI
        // We trim the local path to match the keys in our dictionary (e.g., "fakturakraj.json")
        string path = request.RequestUri?.LocalPath.TrimStart('/').ToLower() ?? string.Empty;

        // Find a matching endpoint in our map (checking if the URL ends with the defined key)
        var keySelector = EndpointMap.FirstOrDefault(x => path.EndsWith(x.Key, StringComparison.OrdinalIgnoreCase));

        // If no matching endpoint is found, proceed without signing
        if (keySelector.Value is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // 2. Retrieve the specific Hex Key from configuration
        string? keyHex = keySelector.Value(_config);

        if (string.IsNullOrEmpty(keyHex))
        {
            throw new InvalidOperationException($"API Key is missing in configuration for endpoint: {path}");
        }

        // 3. Prepare the request content for hashing
        string requestContent = string.Empty;
        if (request.Content != null)
        {
            requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        // 4. Compute HMAC-SHA1 Signature
        byte[] keyBytes = Convert.FromHexString(keyHex);
        using var hmac = new HMACSHA1(keyBytes);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestContent));

        string hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        // 5. Add the custom Authentication header
        request.Headers.TryAddWithoutValidation(
            "Authentication",
            $"IAPIS user={_config.User}, hmac-sha1={hash}");

        // 6. Send the request down the pipeline
        return await base.SendAsync(request, cancellationToken);
    }
}
