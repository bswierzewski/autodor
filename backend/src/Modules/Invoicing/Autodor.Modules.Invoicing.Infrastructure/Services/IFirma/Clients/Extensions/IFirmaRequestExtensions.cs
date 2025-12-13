using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;

/// <summary>
/// Extension methods for configuring iFirma authentication on HTTP requests.
/// These methods provide a clean API for setting and retrieving the API key type
/// without exposing implementation details like HttpRequestOptions.
/// </summary>
public static class IFirmaRequestExtensions
{
    /// <summary>
    /// Unique key used to store the iFirma API key type in the HTTP request options.
    /// This uses HttpRequestOptionsKey to safely associate the enum with the request.
    /// </summary>
    private static readonly HttpRequestOptionsKey<IFirmaKeyType?> AuthKeyOption =
        new("IFirmaAuthKey");

    /// <summary>
    /// Sets the iFirma API key type for the given HTTP request.
    /// The authentication handler will use this information to select the correct
    /// secret key from configuration and compute the HMAC signature.
    /// </summary>
    /// <param name="request">The HTTP request message to configure.</param>
    /// <param name="keyType">The type of API key to use for this request.</param>
    /// <returns>The same request object for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    public static HttpRequestMessage SetApiKey(this HttpRequestMessage request, IFirmaKeyType keyType)
    {
        if (request == null)
            ArgumentNullException.ThrowIfNull(request);

        request.Options.Set(AuthKeyOption, keyType);
        return request;
    }

    /// <summary>
    /// Retrieves the iFirma API key type associated with the given HTTP request.
    /// Returns null if no key type has been set.
    /// </summary>
    /// <param name="request">The HTTP request message to query.</param>
    /// <returns>The API key type if set; otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    public static IFirmaKeyType? GetApiKey(this HttpRequestMessage request)
    {
        if (request == null)
            ArgumentNullException.ThrowIfNull(request);

        if (request.Options.TryGetValue(AuthKeyOption, out IFirmaKeyType? keyType))        
            return keyType;        

        return null;
    }
}
