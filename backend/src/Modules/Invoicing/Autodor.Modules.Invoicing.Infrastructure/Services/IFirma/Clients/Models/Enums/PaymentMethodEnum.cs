namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;

/// <summary>
/// Payment method specification.
/// </summary>
public enum PaymentMethodEnum
{
    /// <summary>
    /// Cash
    /// </summary>
    GTK,

    /// <summary>
    /// Cash on delivery
    /// </summary>
    POB,

    /// <summary>
    /// Bank transfer
    /// </summary>
    PRZ,

    /// <summary>
    /// Card
    /// </summary>
    KAR,

    /// <summary>
    /// Direct debit
    /// </summary>
    PZA,

    /// <summary>
    /// Check
    /// </summary>
    CZK,

    /// <summary>
    /// Compensation
    /// </summary>
    KOM,

    /// <summary>
    /// Barter
    /// </summary>
    BAR,

    /// <summary>
    /// DotPay
    /// </summary>
    DOT,

    /// <summary>
    /// PayPal
    /// </summary>
    PAL,

    /// <summary>
    /// PayU
    /// </summary>
    ALG,

    /// <summary>
    /// Przelewy24
    /// </summary>
    P24,

    /// <summary>
    /// tpay.com
    /// </summary>
    TPA,

    /// <summary>
    /// Electronic payment
    /// </summary>
    ELE
}
