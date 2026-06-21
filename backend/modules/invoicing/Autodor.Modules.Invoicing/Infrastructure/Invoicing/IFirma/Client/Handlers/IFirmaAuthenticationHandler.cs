using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Microsoft.Extensions.Options;
using Refit;
using System.Reflection;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Handlers;

/// <summary>
/// Signs outgoing iFirma API requests with the HMAC-SHA1 authentication header.
/// The required API key is selected from metadata attached to the Refit endpoint.
/// </summary>
public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private static readonly HttpRequestOptionsKey<RestMethodInfo> RestMethodInfoKey =
        new(HttpRequestMessageOptions.RestMethodInfo);

    private readonly IFirmaOptions _options = options.Value;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri ?? throw new InvalidOperationException("Brakuje adresu żądania API iFirma.");
        var keyType = GetKeyType(request);
        var (keyName, key) = GetKeyDetails(keyType);
        var content = request.Content is not null
            ? await request.Content.ReadAsStringAsync(cancellationToken)
            : string.Empty;

        var message = $"{uri.GetLeftPart(UriPartial.Path)}{_options.User}{keyName}{content}";
        var signature = HmacSha1.Compute(key, message);

        request.Headers.Add("Authentication", $"IAPIS user={_options.User}, hmac-sha1={signature}");

        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Reads the required API key type from the attribute applied to the Refit method.
    /// </summary>
    private static IFirmaKeyType GetKeyType(HttpRequestMessage request)
    {
        if (!request.Options.TryGetValue(RestMethodInfoKey, out var restMethodInfo))
            throw new InvalidOperationException("Brakuje informacji o metodzie Refit dla żądania API iFirma.");

        return restMethodInfo.MethodInfo.GetCustomAttribute<IFirmaKeyAttribute>()?.KeyType
            ?? throw new InvalidOperationException(
                $"Metoda {restMethodInfo.MethodInfo.Name} nie określa klucza API iFirma.");
    }

    /// <summary>
    /// Resolves the iFirma key name and configured secret for the requested key type.
    /// </summary>
    private (string Name, string Key) GetKeyDetails(IFirmaKeyType keyType) =>
        keyType switch
        {
            IFirmaKeyType.Invoice => ("faktura", GetKey(_options.ApiKeys.Faktura)),
            IFirmaKeyType.Subscriber => ("abonent", GetKey(_options.ApiKeys.Abonent)),
            IFirmaKeyType.Account => ("rachunek", GetKey(_options.ApiKeys.Rachunek)),
            IFirmaKeyType.Expense => ("wydatek", GetKey(_options.ApiKeys.Wydatek)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(keyType),
                keyType,
                "Nieobsługiwany typ klucza API iFirma.")
        };

    /// <summary>
    /// Returns a configured API key and rejects missing or empty values.
    /// </summary>
    private static string GetKey(string? key) =>
        string.IsNullOrWhiteSpace(key)
            ? throw new InvalidOperationException("Brakuje klucza API w konfiguracji.")
            : key;
}
