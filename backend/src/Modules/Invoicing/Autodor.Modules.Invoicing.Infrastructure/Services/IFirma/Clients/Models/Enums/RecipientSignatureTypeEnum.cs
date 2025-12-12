namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;

/// <summary>
/// Recipient signature type specification.
/// </summary>
public enum RecipientSignatureTypeEnum
{
    /// <summary>
    /// Authorized person to receive VAT invoice
    /// </summary>
    OUP,

    /// <summary>
    /// Authorization
    /// </summary>
    UPO,

    /// <summary>
    /// Without recipient signature
    /// </summary>
    BPO,

    /// <summary>
    /// Without recipient and issuer signature
    /// </summary>
    BWO
}
