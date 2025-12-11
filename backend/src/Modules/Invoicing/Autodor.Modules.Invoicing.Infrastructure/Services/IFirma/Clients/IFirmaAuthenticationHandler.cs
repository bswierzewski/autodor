using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

/// <summary>
/// A DelegatingHandler that automatically signs HTTP requests with iFirma specific HMAC-SHA1 headers.
/// It uses a prefix-based routing map to determine which API Key to use based on the request URL path.
/// Routes are matched in the order they are defined.
/// </summary>
public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private readonly IFirmaOptions _config = options.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Internal record to define route mappings.
    /// </summary>
    /// <param name="KeyName">The specific key identifier used in the signature (e.g., "faktura").</param>
    /// <param name="KeySelector">Function to select the hex API key from options.</param>
    /// <param name="Prefixes">List of URL path prefixes that trigger this key usage.</param>
    private record RouteConfig(string KeyName, Func<IFirmaOptions, string?> KeySelector, string[] Prefixes);

    /// <summary>
    /// Route configuration for iFirma API endpoints.
    /// Maps key names to their selectors and associated URL prefixes.
    /// Routes are checked in order, so more specific routes should be listed first.
    /// </summary>
    private static readonly IReadOnlyList<RouteConfig> Routes =
        [
            // Invoices (Faktura)
            new("faktura", opt => opt.Keys.Faktura, [
                    "/iapi/fakturakraj"
                ]),

            //// Contractors (Abonent)
            //("Abonent", opt => opt.Keys.Abonent, ["/iapi/abonent"]),

            //// Bills (Rachunek)
            //("Rachunek", opt => opt.Keys.Rachunek, ["/iapi/rachunek"]),
        ];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestPath = request.RequestUri?.LocalPath ?? string.Empty;
        var matchedRoute = FindMatchingRoute(requestPath);

        if (matchedRoute is null)
            return await base.SendAsync(request, cancellationToken);

        string? keyHex = matchedRoute.KeySelector(_config);
        if (string.IsNullOrEmpty(keyHex))
            throw new InvalidOperationException($"API Key is missing in configuration for key name: {matchedRoute.KeyName}");

        string requestContent = await GetContentAsync(request, cancellationToken);
        string messageToSign = BuildMessageToSign(requestPath, matchedRoute.KeyName, requestContent);

        string hash = HmacSha1.Compute(keyHex, messageToSign);
        string headerValue = $"IAPIS user={_config.User}, hmac-sha1={hash}";

        request.Headers.TryAddWithoutValidation("Authentication", headerValue);

        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Finds the first route configuration where one of the prefixes matches the start of the request path.
    /// Comparison is case-insensitive.
    /// </summary>
    private static RouteConfig? FindMatchingRoute(string path)
        => Routes.FirstOrDefault(r => r.Prefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// Constructs the concatenation string required for the HMAC signature.
    /// Format: BaseUrl + Path + User + KeyName + Content
    /// </summary>
    private string BuildMessageToSign(string path, string keyName, string content)
        => $"{_config.BaseUrl.TrimEnd('/')}{path}{_config.User}{keyName}{content}";

    /// <summary>
    /// Safely reads the request content as a string. Returns empty string if content is null.
    /// </summary>
    private static async Task<string> GetContentAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content is null)
            return string.Empty;

        return await request.Content.ReadAsStringAsync(cancellationToken);
    }
}