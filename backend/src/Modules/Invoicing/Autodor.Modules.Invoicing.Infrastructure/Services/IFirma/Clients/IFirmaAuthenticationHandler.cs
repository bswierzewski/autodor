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
    private readonly IFirmaOptions _options = options.Value
        ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Internal record to define route mappings.
    /// </summary>
    private record RouteConfig(
        string KeyName,
        Func<IFirmaOptions, string?> KeySelector,
        string[] Prefixes);

    /// <summary>
    /// Route configuration for iFirma API endpoints.
    /// Maps key names to their selectors and associated URL prefixes.
    /// Routes are checked in order, so more specific routes should be listed first.
    /// </summary>
    private static readonly IReadOnlyList<RouteConfig> Routes =
    [
        new("faktura", opt => opt.ApiKeys.Faktura, [
            "/iapi/fakturakraj"
        ]),
    ];

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string path = request.RequestUri?.LocalPath ?? string.Empty;
        var route = FindMatchingRoute(path);

        if (route is not null)
        {
            string content = await GetContentAsync(request, cancellationToken);
            string signature = ComputeSignature(route, path, content);

            request.Headers.Add(
                "Authentication",
                $"IAPIS user={_options.User}, hmac-sha1={signature}"
            );
        }

        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Constructs the concatenation string required for the HMAC signature.
    /// Format: BaseUrl + Path + User + KeyName + Content
    /// </summary>
    private string ComputeSignature(RouteConfig route, string path, string content)
    {
        string keyHex = route.KeySelector(_options)
            ?? throw new InvalidOperationException($"Missing API key for {route.KeyName}");

        var baseUrl = _options.BaseUrl?.TrimEnd('/') ?? string.Empty;
        var user = _options.User ?? string.Empty;
        var keyName = route.KeyName;

        string message = $"{baseUrl}{path}{user}{keyName}{content}";
        return HmacSha1.Compute(keyHex, message);
    }

    /// <summary>
    /// Finds the first route configuration where one of the prefixes matches the start of the request path.
    /// Comparison is case-insensitive.
    /// </summary>
    private static RouteConfig? FindMatchingRoute(string path) =>
        Routes.FirstOrDefault(r =>
            r.Prefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// Safely reads the request content as a string. Returns empty string if content is null.
    /// </summary>
    private static Task<string> GetContentAsync(HttpRequestMessage request, CancellationToken ct) =>
        request.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(string.Empty);
}