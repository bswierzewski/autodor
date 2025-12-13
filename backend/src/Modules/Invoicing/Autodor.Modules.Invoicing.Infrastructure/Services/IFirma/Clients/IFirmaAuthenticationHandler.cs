using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

/// <summary>
/// A DelegatingHandler that automatically signs HTTP requests with iFirma-specific HMAC-SHA1 headers.
/// It retrieves the API key type from the request (set via SetApiKey extension method),
/// resolves it to the corresponding secret key from configuration, and computes the HMAC signature.
/// This approach provides compile-time safety by using an enum instead of string-based routing.
/// </summary>
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

    /// <summary>
    /// Maps the IFirmaKeyType enum to a pair of (KeyName, KeyHex).
    /// KeyName is the string identifier used in the iFirma API signature (e.g., "faktura").
    /// KeyHex is the secret API key retrieved from configuration.
    /// </summary>
    /// <param name="type">The API key type to resolve.</param>
    /// <returns>A tuple containing the key name and the hex-encoded secret key.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported key type is provided.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the API key is missing from configuration.</exception>
    private (string KeyName, string KeyHex) ResolveKeyDetails(IFirmaKeyType type)
    {
        return type switch
        {
            // The key name "faktura" is required by iFirma in the HMAC signature.
            // The value is the secret key from appsettings configuration.
            IFirmaKeyType.Invoice => ("faktura", EnsureKey(_options.ApiKeys.Faktura, type)),

            IFirmaKeyType.Subscriber => ("abonent", EnsureKey(_options.ApiKeys.Abonent, type)),

            IFirmaKeyType.Account => ("rachunek", EnsureKey(_options.ApiKeys.Rachunek, type)),

            IFirmaKeyType.Expense => ("wydatek", EnsureKey(_options.ApiKeys.Wydatek, type)),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type,
                "Unsupported iFirma key type. Ensure the enum value is mapped in ResolveKeyDetails.")
        };
    }

    /// <summary>
    /// Validates that the API key is present and non-empty.
    /// </summary>
    /// <param name="key">The API key value from configuration.</param>
    /// <param name="type">The API key type (used for error reporting).</param>
    /// <returns>The API key if valid.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the API key is null or whitespace.</exception>
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

    /// <summary>
    /// Computes the HMAC-SHA1 signature required by the iFirma API.
    /// The message format is: Url + User + KeyName + Content
    /// </summary>
    /// <param name="keyName">The API key name (e.g., "faktura") used in the signature.</param>
    /// <param name="keyHex">The secret API key in hex format.</param>
    /// <param name="requestUri">The full request URI including the path.</param>
    /// <param name="content">The request body content (empty string if no body).</param>
    /// <returns>The computed HMAC-SHA1 signature as a hex string.</returns>
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

    /// <summary>
    /// Safely reads the request body content as a string.
    /// Returns an empty string if no content is present.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="ct">Cancellation token for the async operation.</param>
    /// <returns>The request body as a string, or an empty string if no body exists.</returns>
    private static Task<string> GetContentAsync(HttpRequestMessage request, CancellationToken ct) =>
        request.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(string.Empty);
}
