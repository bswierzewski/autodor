namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;

/// <summary>
/// Specifies the authorization key required by an iFirma endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class IFirmaKeyAttribute(IFirmaKeyType keyType) : Attribute
{
    public IFirmaKeyType KeyType { get; } = keyType;
}
