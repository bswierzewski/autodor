namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;

/// <summary>
/// Enum defining the types of API keys supported by iFirma.
/// Each type maps to a specific API endpoint and secret key configuration.
/// </summary>
public enum IFirmaKeyType
{
    /// <summary>
    /// API key for domestic invoice operations (faktury krajowe).
    /// </summary>
    Invoice,

    /// <summary>
    /// API key for subscriber/recurring billing operations.
    /// </summary>
    Subscriber,

    /// <summary>
    /// API key for invoice/billing account operations.
    /// </summary>
    Account,

    /// <summary>
    /// API key for expense operations (wydatki).
    /// </summary>
    Expense,

    // Add additional key types as needed when iFirma supports them
}
