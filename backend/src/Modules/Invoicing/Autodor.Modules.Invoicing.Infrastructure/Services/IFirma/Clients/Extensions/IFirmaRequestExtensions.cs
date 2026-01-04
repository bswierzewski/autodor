using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;

public static class IFirmaRequestExtensions
{
    private static readonly HttpRequestOptionsKey<IFirmaKeyType?> AuthKeyOption =
        new("IFirmaAuthKey");

    public static HttpRequestMessage SetApiKey(this HttpRequestMessage request, IFirmaKeyType keyType)
    {
        if (request == null)
            ArgumentNullException.ThrowIfNull(request);

        request.Options.Set(AuthKeyOption, keyType);
        return request;
    }

    public static IFirmaKeyType? GetApiKey(this HttpRequestMessage request)
    {
        if (request == null)
            ArgumentNullException.ThrowIfNull(request);

        if (request.Options.TryGetValue(AuthKeyOption, out IFirmaKeyType? keyType))        
            return keyType;        

        return null;
    }
}
