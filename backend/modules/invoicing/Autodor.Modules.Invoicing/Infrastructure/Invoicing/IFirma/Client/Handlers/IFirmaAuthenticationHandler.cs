using System.Reflection;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Microsoft.Extensions.Options;
using Refit;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Handlers;

/// <summary>
/// Signs outgoing iFirma requests with the key declared on the invoked endpoint.
/// </summary>
public class IFirmaAuthenticationHandler(IOptions<IFirmaOptions> options) : DelegatingHandler
{
    private static readonly HttpRequestOptionsKey<RestMethodInfo> RestMethodInfoKey =
        new(HttpRequestMessageOptions.RestMethodInfo);
    private static readonly HttpRequestOptionsKey<string> MethodNameKey =
        new(HttpRequestMessageOptions.MethodName);

    private readonly IFirmaOptions _options = options.Value;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri ?? throw new InvalidOperationException("Brakuje adresu żądania API iFirma.");
        var (keyName, key) = GetKeyDetails(GetKeyType(request));
        var content = request.Content is not null
            ? await request.Content.ReadAsStringAsync(cancellationToken)
            : string.Empty;

        var message = $"{uri.GetLeftPart(UriPartial.Path)}{_options.User}{keyName}{content}";
        var signature = HmacSha1.Compute(key, message);
        request.Headers.Add("Authentication", $"IAPIS user={_options.User}, hmac-sha1={signature}");

        return await base.SendAsync(request, cancellationToken);
    }

    private static IFirmaKeyType GetKeyType(HttpRequestMessage request)
    {
        var method = request.Options.TryGetValue(RestMethodInfoKey, out var restMethodInfo)
            ? restMethodInfo.MethodInfo
            : request.Options.TryGetValue(MethodNameKey, out var methodName)
                ? typeof(IIFirmaHttpClient).GetMethods().SingleOrDefault(candidate => candidate.Name == methodName)
                : null;

        if (method is null)
            throw new InvalidOperationException("Brakuje informacji o metodzie Refit dla żądania API iFirma.");

        return method.GetCustomAttribute<IFirmaKeyAttribute>()?.KeyType
            ?? throw new InvalidOperationException(
                $"Metoda {method.Name} nie określa klucza API iFirma.");
    }

    private (string Name, string Key) GetKeyDetails(IFirmaKeyType keyType) => keyType switch
    {
        IFirmaKeyType.Subscriber => ("abonent", GetKey(_options.ApiKeys.Abonent)),
        IFirmaKeyType.Invoice => ("faktura", GetKey(_options.ApiKeys.Faktura)),
        IFirmaKeyType.Bill => ("rachunek", GetKey(_options.ApiKeys.Rachunek)),
        IFirmaKeyType.Expense => ("wydatek", GetKey(_options.ApiKeys.Wydatek)),
        _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, "Nieobsługiwany typ klucza API iFirma.")
    };

    private static string GetKey(string? key) =>
        string.IsNullOrWhiteSpace(key)
            ? throw new InvalidOperationException("Brakuje klucza API w konfiguracji iFirma.")
            : key;
}
