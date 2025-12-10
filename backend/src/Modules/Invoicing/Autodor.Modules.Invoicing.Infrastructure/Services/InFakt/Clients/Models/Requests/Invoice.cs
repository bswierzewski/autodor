using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

/// <summary>
/// Model representing an invoice in InFakt API.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Invoice number.
    /// Optional. If not provided, will be auto-generated based on "Next invoice number" setting.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Currency code.
    /// Optional, defaults to PLN. Valid values: THB, USD, AUD, HKD, CAD, NZD, SGD, EUR, HUF, CHF, GBP, UAH, JPY, CZK, DKK, ISK, NOK, SEK, HRK, RON, BGN, TRY, LTL, LVL, PHP, MXN, ZAR, BRL, MYR, RUB, IDR, KRW, CNY, INR.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Amount paid in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("paid_price")]
    public int? PaidPrice { get; set; }

    /// <summary>
    /// Invoice notes.
    /// Optional.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Invoice kind.
    /// Optional. Values: proforma (Proforma Invoice), vat (VAT Invoice).
    /// </summary>
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    /// <summary>
    /// Payment method.
    /// Optional. Values: transfer, cash, card, barter, check, bill_of_sale, delivery, compensation, accredited, paypal, instalment_sale, payu, tpay, przelewy24, dotpay, other.
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Split payment availability flag.
    /// Optional.
    /// </summary>
    [JsonPropertyName("split_payment")]
    public bool? SplitPayment { get; set; }

    /// <summary>
    /// Split payment type (when split payment is used).
    /// Optional. Values: required, optional.
    /// </summary>
    [JsonPropertyName("split_payment_type")]
    public string? SplitPaymentType { get; set; }

    /// <summary>
    /// Recipient's signature (name).
    /// Optional.
    /// </summary>
    [JsonPropertyName("recipient_signature")]
    public string? RecipientSignature { get; set; }

    /// <summary>
    /// Seller's signature (name).
    /// Optional.
    /// </summary>
    [JsonPropertyName("seller_signature")]
    public string? SellerSignature { get; set; }

    /// <summary>
    /// Invoice issue date in format YYYY-MM-DD.
    /// Optional.
    /// </summary>
    [JsonPropertyName("invoice_date")]
    public string? InvoiceDate { get; set; }

    /// <summary>
    /// Sale date in format YYYY-MM-DD.
    /// Optional.
    /// </summary>
    [JsonPropertyName("sale_date")]
    public string? SaleDate { get; set; }

    /// <summary>
    /// Payment deadline in format YYYY-MM-DD.
    /// Optional.
    /// </summary>
    [JsonPropertyName("payment_date")]
    public string? PaymentDate { get; set; }

    /// <summary>
    /// Date of payment in format YYYY-MM-DD.
    /// Optional.
    /// </summary>
    [JsonPropertyName("paid_date")]
    public string? PaidDate { get; set; }

    /// <summary>
    /// Total net price in groszy (cents).
    /// Optional, calculated automatically.
    /// </summary>
    [JsonPropertyName("net_price")]
    public int? NetPrice { get; set; }

    /// <summary>
    /// VAT tax amount in groszy (cents).
    /// Optional, calculated automatically.
    /// </summary>
    [JsonPropertyName("tax_price")]
    public int? TaxPrice { get; set; }

    /// <summary>
    /// Total gross price in groszy (cents).
    /// Optional, calculated automatically.
    /// </summary>
    [JsonPropertyName("gross_price")]
    public int? GrossPrice { get; set; }

    /// <summary>
    /// Amount left to pay in groszy (cents).
    /// Optional, calculated automatically.
    /// </summary>
    [JsonPropertyName("left_to_pay")]
    public int? LeftToPay { get; set; }

    /// <summary>
    /// Check for duplicate invoice numbers.
    /// Optional, defaults to false.
    /// </summary>
    [JsonPropertyName("check_duplicate_number")]
    public bool? CheckDuplicateNumber { get; set; }

    /// <summary>
    /// Bank name (for transfer payment method).
    /// Optional.
    /// </summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>
    /// Bank account number (for transfer payment method).
    /// Optional.
    /// </summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>
    /// SWIFT code (for transfer payment method).
    /// Optional.
    /// </summary>
    [JsonPropertyName("swift")]
    public string? Swift { get; set; }

    /// <summary>
    /// Sale type for foreign clients.
    /// Optional. Values: service, merchandise.
    /// </summary>
    [JsonPropertyName("sale_type")]
    public string? SaleType { get; set; }

    /// <summary>
    /// Invoice date kind.
    /// Optional. Values: sale_date, service_date, cargo_date, continuous_date_end_on.
    /// </summary>
    [JsonPropertyName("invoice_date_kind")]
    public string? InvoiceDateKind { get; set; }

    /// <summary>
    /// Continuous service start date in format YYYY-MM-DD.
    /// Required when invoice_date_kind is continuous_date_end_on.
    /// </summary>
    [JsonPropertyName("continuous_service_start_on")]
    public string? ContinuousServiceStartOn { get; set; }

    /// <summary>
    /// Continuous service end date in format YYYY-MM-DD.
    /// Required when invoice_date_kind is continuous_date_end_on.
    /// </summary>
    [JsonPropertyName("continuous_service_end_on")]
    public string? ContinuousServiceEndOn { get; set; }

    /// <summary>
    /// Invoice line items (services/products).
    /// Required.
    /// </summary>
    [JsonPropertyName("services")]
    public required ICollection<InvoiceItem> Services { get; set; }

    /// <summary>
    /// Client/buyer information.
    /// Required.
    /// </summary>
    [JsonPropertyName("client")]
    public required Client Client { get; set; }

    /// <summary>
    /// VAT exemption reason ID.
    /// Optional. Required for VAT-exempt sales if no default exemption is set.
    /// </summary>
    [JsonPropertyName("vat_exemption_reason")]
    public int? VatExemptionReason { get; set; }

    /// <summary>
    /// BDO registration number.
    /// Optional.
    /// </summary>
    [JsonPropertyName("bdo_code")]
    public string? BdoCode { get; set; }

    /// <summary>
    /// Transaction type ID.
    /// Optional.
    /// </summary>
    [JsonPropertyName("transaction_kind_id")]
    public int? TransactionKindId { get; set; }

    /// <summary>
    /// Array of document marking IDs.
    /// Optional.
    /// </summary>
    [JsonPropertyName("document_markings_ids")]
    public int[]? DocumentMarkingIds { get; set; }

    /// <summary>
    /// Receipt number.
    /// Optional. Send empty string to remove.
    /// </summary>
    [JsonPropertyName("receipt_number")]
    public string? ReceiptNumber { get; set; }

    /// <summary>
    /// Do not include in income tax.
    /// Optional. Required for receipt invoices, corrections, or advances.
    /// </summary>
    [JsonPropertyName("not_income")]
    public bool? NotIncome { get; set; }

    /// <summary>
    /// VAT exchange date kind for foreign currency invoices.
    /// Optional. Values: vat, pit.
    /// </summary>
    [JsonPropertyName("vat_exchange_date_kind")]
    public string? VatExchangeDateKind { get; set; }
}
