namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;

/// <summary>
/// Identifiers of authorization keys supported by the iFirma API.
/// </summary>
public enum IFirmaKeyType
{
    /// <summary>Key used to read and change the accounting month.</summary>
    Subscriber,

    /// <summary>Key used for invoice operations.</summary>
    Invoice,

    /// <summary>Key used for bill operations.</summary>
    Bill,

    /// <summary>Key used for expense operations.</summary>
    Expense
}
