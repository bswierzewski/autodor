using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;

/// <summary>
/// Model representing a domestic invoice in iFirma API.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Amount paid for the invoice.
    /// Required, >= 0.00 and <= gross invoice total, <= 10 digits, < 100000000.
    /// </summary>
    [JsonPropertyName("Zaplacono")]
    public decimal Paid { get; set; }

    /// <summary>
    /// Amount paid on the document.
    /// Required, >= 0.00 and <= gross invoice total, <= 10 digits, < 100000000.
    /// </summary>
    [JsonPropertyName("ZaplaconoNaDokumencie")]
    public decimal PaidOnDocument { get; set; }

    /// <summary>
    /// Calculation method: NET (from net) or BRT (from gross).
    /// Required.
    /// </summary>
    [JsonPropertyName("LiczOd")]
    public required string CalculationType { get; set; }

    /// <summary>
    /// Bank account number.
    /// Optional, up to 28 characters. Use "BRAK" if account should not be displayed.
    /// </summary>
    [JsonPropertyName("NumerKontaBankowego")]
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// Split Payment mechanism flag.
    /// Optional. Uses account settings if not specified.
    /// </summary>
    [JsonPropertyName("SplitPayment")]
    public bool? SplitPayment { get; set; }

    /// <summary>
    /// Invoice issue date.
    /// Required, format: YYYY-MM-DD.
    /// Must be >= sales date, >= previous invoice date, <= 15th day of next month relative to sales date.
    /// </summary>
    [JsonPropertyName("DataWystawienia")]
    public required string IssueDate { get; set; }

    /// <summary>
    /// Invoice issue place.
    /// Optional, up to 50 characters.
    /// </summary>
    [JsonPropertyName("MiejsceWystawienia")]
    public string? IssuePlace { get; set; }

    /// <summary>
    /// Sales date of goods or services.
    /// Required, format: YYYY-MM-DD.
    /// If current accounting month, use current date, otherwise last day of month.
    /// </summary>
    [JsonPropertyName("DataSprzedazy")]
    public required string SalesDate { get; set; }

    /// <summary>
    /// Sales date format: DZN (daily) or MSC (monthly).
    /// Required.
    /// </summary>
    [JsonPropertyName("FormatDatySprzedazy")]
    public string? SalesDateFormat { get; set; }

    /// <summary>
    /// Payment deadline.
    /// Optional, format: YYYY-MM-DD. Must be >= sales date.
    /// </summary>
    [JsonPropertyName("TerminPlatnosci")]
    public string? PaymentDeadline { get; set; }

    /// <summary>
    /// Payment method.
    /// Required. Valid values: GTK, POB, PRZ, KAR, PZA, CZK, KOM, BAR, DOT, PAL, ALG, P24, TPA, ELE.
    /// </summary>
    [JsonPropertyName("SposobZaplaty")]
    public required string PaymentMethod { get; set; }

    /// <summary>
    /// Numbering series name.
    /// Optional. Uses default if not provided.
    /// </summary>
    [JsonPropertyName("NazwaSeriiNumeracji")]
    public string? NumberingSeriesName { get; set; }

    /// <summary>
    /// Template name.
    /// Optional. Uses default if not provided.
    /// </summary>
    [JsonPropertyName("NazwaSzablonu")]
    public string? TemplateName { get; set; }

    /// <summary>
    /// Recipient signature type.
    /// Required. Valid values: OUP, UPO, BPO, BWO.
    /// </summary>
    [JsonPropertyName("RodzajPodpisuOdbiorcy")]
    public string? RecipientSignatureType { get; set; }

    /// <summary>
    /// Recipient signature.
    /// Optional, up to 70 characters.
    /// </summary>
    [JsonPropertyName("PodpisOdbiorcy")]
    public string? RecipientSignature { get; set; }

    /// <summary>
    /// Issuer signature.
    /// Optional, up to 70 characters.
    /// </summary>
    [JsonPropertyName("PodpisWystawcy")]
    public string? IssuerSignature { get; set; }

    /// <summary>
    /// Invoice notes.
    /// Optional, up to 1000 characters.
    /// </summary>
    [JsonPropertyName("Uwagi")]
    public string? Notes { get; set; }

    /// <summary>
    /// Visibility of BDO/GIOŚ number.
    /// Required. Can use WidocznyNumerGios or WidocznyNumerBdo.
    /// </summary>
    [JsonPropertyName("WidocznyNumerGios")]
    public bool VisibleGiosNumber { get; set; }

    /// <summary>
    /// Invoice number.
    /// Required, up to 10 characters. Use null for auto-generated number from series.
    /// </summary>
    [JsonPropertyName("Numer")]
    public int? Number { get; set; }

    /// <summary>
    /// Contractor identifier.
    /// Optional, up to 15 characters.
    /// </summary>
    [JsonPropertyName("IdentyfikatorKontrahenta")]
    public string? ContractorIdentifier { get; set; }

    /// <summary>
    /// Contractor EU prefix.
    /// Optional, up to 2 characters.
    /// </summary>
    [JsonPropertyName("PrefiksUEKontrahenta")]
    public string? ContractorEUPrefix { get; set; }

    /// <summary>
    /// Contractor VAT number.
    /// Optional, up to 13 characters.
    /// </summary>
    [JsonPropertyName("NIPKontrahenta")]
    public string? ContractorVatNumber { get; set; }

    /// <summary>
    /// Invoice line items.
    /// Required. Collection of invoice items.
    /// </summary>
    [JsonPropertyName("Pozycje")]
    public required ICollection<InvoiceItem> Items { get; set; }

    /// <summary>
    /// Contractor information.
    /// Required.
    /// </summary>
    [JsonPropertyName("Kontrahent")]
    public required Contractor Contractor { get; set; }
}
