using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Responses;

/// <summary>
/// Response model from InFakt API after creating or retrieving an invoice.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Unique identifier of the invoice.
    /// Read-only.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Invoice number.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Currency code (default: PLN).
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Amount paid in groszy (cents).
    /// </summary>
    [JsonPropertyName("paid_price")]
    public int? PaidPrice { get; set; }

    /// <summary>
    /// Invoice notes.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Invoice kind (proforma, vat, etc.).
    /// </summary>
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    /// <summary>
    /// Payment method.
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Split payment availability.
    /// </summary>
    [JsonPropertyName("split_payment")]
    public bool? SplitPayment { get; set; }

    /// <summary>
    /// Split payment type if enabled.
    /// </summary>
    [JsonPropertyName("split_payment_type")]
    public string? SplitPaymentType { get; set; }

    /// <summary>
    /// Recipient's signature.
    /// </summary>
    [JsonPropertyName("recipient_signature")]
    public string? RecipientSignature { get; set; }

    /// <summary>
    /// Seller's signature.
    /// </summary>
    [JsonPropertyName("seller_signature")]
    public string? SellerSignature { get; set; }

    /// <summary>
    /// Invoice issue date in format YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("invoice_date")]
    public string? InvoiceDate { get; set; }

    /// <summary>
    /// Sale date in format YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("sale_date")]
    public string? SaleDate { get; set; }

    /// <summary>
    /// Invoice status.
    /// Read-only. Values: draft, sent, printed, paid.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Payment deadline in format YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("payment_date")]
    public string? PaymentDate { get; set; }

    /// <summary>
    /// Date of payment in format YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("paid_date")]
    public string? PaidDate { get; set; }

    /// <summary>
    /// Total net price in groszy (cents).
    /// </summary>
    [JsonPropertyName("net_price")]
    public int? NetPrice { get; set; }

    /// <summary>
    /// VAT tax amount in groszy (cents).
    /// </summary>
    [JsonPropertyName("tax_price")]
    public int? TaxPrice { get; set; }

    /// <summary>
    /// Total gross price in groszy (cents).
    /// </summary>
    [JsonPropertyName("gross_price")]
    public int? GrossPrice { get; set; }

    /// <summary>
    /// Amount left to pay in groszy (cents).
    /// </summary>
    [JsonPropertyName("left_to_pay")]
    public int? LeftToPay { get; set; }

    /// <summary>
    /// Client ID.
    /// </summary>
    [JsonPropertyName("client_id")]
    public int? ClientId { get; set; }

    /// <summary>
    /// BDO registration number.
    /// </summary>
    [JsonPropertyName("bdo_code")]
    public string? BdoCode { get; set; }

    /// <summary>
    /// KSeF (National Invoice Registry) data.
    /// Read-only.
    /// </summary>
    [JsonPropertyName("ksef_data")]
    public object? KsefData { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// Read-only.
    /// </summary>
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
}
