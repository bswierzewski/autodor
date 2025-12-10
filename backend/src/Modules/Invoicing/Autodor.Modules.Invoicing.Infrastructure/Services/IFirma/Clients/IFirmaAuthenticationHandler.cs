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
    private readonly IFirmaOptions _config = options.Value;

    /// <summary>
    /// Routes for iFirma API endpoints.
    /// Each route maps a URL prefix to a configuration key selector.
    /// Routes are checked in order, so more specific routes should be listed first.
    /// </summary>
    private static readonly IReadOnlyList<(string prefix, Func<IFirmaOptions, string?> keySelector)> EndpointRoutes =
        [
            // Invoices (Faktura)
            ("/iapi/fakturakraj", opt => opt.Keys.Faktura),

            //// Contractors (Abonent)
            //("/iapi/abonent", opt => opt.Keys.Abonent),

            //// Bills (Rachunek)
            //("/iapi/rachunek", opt => opt.Keys.Rachunek),
        ];

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Extract and normalize the request path
        string path = request.RequestUri?.LocalPath.ToLower() ?? string.Empty;

        // 2. Find matching route based on URL prefix
        // Routes are checked in order, so more specific routes should be listed first
        var route = EndpointRoutes.FirstOrDefault(r => path.StartsWith(r.prefix, StringComparison.OrdinalIgnoreCase));

        // If no matching route is found, proceed without signing
        if (route == default)
            return await base.SendAsync(request, cancellationToken);

        // 3. Retrieve the specific Hex Key from configuration
        string? keyHex = route.keySelector(_config);

        if (string.IsNullOrEmpty(keyHex))
            throw new InvalidOperationException($"API Key is missing in configuration for endpoint: {route.prefix}");

        // 4. Prepare the request content for hashing
        string requestContent = string.Empty;
        if (request.Content != null)
            requestContent = await request.Content.ReadAsStringAsync(cancellationToken);

        // 5. Compute HMAC-SHA1 Signature using iFirma specification
        // Message format: path + userName + keyName + requestContent
        string message = $"{path}{_config.User}{route.prefix}{requestContent}";
        string hash = HmacSha1.Compute(keyHex, message);

        // 6. Add the custom Authentication header
        request.Headers.TryAddWithoutValidation("Authentication", $"IAPIS user={_config.User}, hmac-sha1={hash}");

        // 7. Send the request down the pipeline
        return await base.SendAsync(request, cancellationToken);
    }
}
