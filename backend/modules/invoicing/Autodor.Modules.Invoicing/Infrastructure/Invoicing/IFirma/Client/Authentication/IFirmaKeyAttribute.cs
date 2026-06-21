namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;

/// <summary>
/// Specifies which iFirma API key must be used to authenticate a Refit endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class IFirmaKeyAttribute(IFirmaKeyType keyType) : Attribute
{
    /// <summary>
    /// Gets the type of API key required by the endpoint.
    /// </summary>
    public IFirmaKeyType KeyType { get; } = keyType;
}
