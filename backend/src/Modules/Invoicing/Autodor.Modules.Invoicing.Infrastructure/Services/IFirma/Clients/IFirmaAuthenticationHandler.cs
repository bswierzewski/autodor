using System.Security.Cryptography;
using System.Text;
using Autodor.Modules.Invoicing.Application.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients
{
    /// <summary>
    /// DelegatingHandler that adds HMAC-SHA1 authentication to IFirma API requests.
    /// Uses modern .NET approaches for hex conversion and pattern matching.
    /// Automatically determines API key type based on URL route mapping.
    /// </summary>
    public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
    {
        private readonly IFirmaOptions _options = options.Value;

        /// <summary>
        /// Route-to-API-key mapping. First matching route wins.
        /// Routes are checked with StartsWith for flexibility.
        /// </summary>
        private static readonly Dictionary<string, IFirmaApiKeyType> RouteMapping = new()
        {
            { "/iapi/faktura", IFirmaApiKeyType.Faktura },
            { "/iapi/kontrahenci", IFirmaApiKeyType.Faktura },
            { "/iapi/abonent", IFirmaApiKeyType.Abonent },
            { "/iapi/rachunek", IFirmaApiKeyType.Rachunek },
            { "/iapi/wydatek", IFirmaApiKeyType.Wydatek }
        };

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Determine API key type from URL path
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var keyType = GetApiKeyTypeFromRoute(path);

            if (keyType == null)
            {
                // No matching route - proceed without authentication header
                return await base.SendAsync(request, cancellationToken);
            }

            var (keyName, keySecret) = GetKeyConfig(keyType.Value);

            // Read request content for HMAC calculation
            string content = request.Content != null
                ? await request.Content.ReadAsStringAsync(cancellationToken)
                : string.Empty;

            // Build HMAC signature: {url}{user}{keyName}{content}
            var uri = request.RequestUri?.AbsoluteUri.Split('?')[0] ?? string.Empty;
            string message = $"{uri}{_options.User}{keyName}{content}";
            string hash = ComputeHmac(message, keySecret);

            // Set IFirma custom Authentication header (not standard Authorization)
            request.Headers.TryAddWithoutValidation(
                "Authentication",
                $"IAPIS user={_options.User}, hmac-sha1={hash}");

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Determines API key type from request path using route mapping.
        /// Returns first matching route (checked with StartsWith).
        /// </summary>
        private static IFirmaApiKeyType? GetApiKeyTypeFromRoute(string path)
        {
            foreach (var (route, keyType) in RouteMapping)
            {
                if (path.StartsWith(route, StringComparison.OrdinalIgnoreCase))
                    return keyType;
            }

            return null;
        }

        /// <summary>
        /// Maps API key type to its configuration (name and secret).
        /// Uses pattern matching for clean, compile-time checked mapping.
        /// </summary>
        private (string Name, string Secret) GetKeyConfig(IFirmaApiKeyType type) => type switch
        {
            IFirmaApiKeyType.Faktura => ("faktura", _options.Faktura),
            IFirmaApiKeyType.Abonent => ("abonent", _options.Abonent),
            IFirmaApiKeyType.Rachunek => ("rachunek", _options.Rachunek),
            IFirmaApiKeyType.Wydatek => ("wydatek", _options.Wydatek),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown API key type")
        };

        /// <summary>
        /// Computes HMAC-SHA1 hash using modern .NET hex conversion methods.
        /// Uses Convert.FromHexString (NET 5+) instead of manual byte array parsing.
        /// </summary>
        private static string ComputeHmac(string message, string hexKey)
        {
            byte[] keyBytes = Convert.FromHexString(hexKey);
            using var hmac = new HMACSHA1(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
