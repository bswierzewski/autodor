using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;

public class InvoiceDto
{
    [JsonPropertyName("Zaplacono")]
    public int Paid { get; set; }

    [JsonPropertyName("ZaplaconoNaDokumencie")]
    public int PaidOnDocument { get; set; }

    [JsonPropertyName("LiczOd")]
    public string CountFrom { get; set; } = string.Empty;

    [JsonPropertyName("NumerKontaBankowego")]
    public string BankAccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("SplitPayment")]
    public string SplitPayment { get; set; } = string.Empty;

    [JsonPropertyName("DataWystawienia")]
    public string IssueDate { get; set; } = string.Empty;

    [JsonPropertyName("MiejsceWystawienia")]
    public string PlaceOfIssue { get; set; } = string.Empty;

    [JsonPropertyName("DataSprzedazy")]
    public string SaleDate { get; set; } = string.Empty;

    [JsonPropertyName("FormatDatySprzedazy")]
    public string SaleDateFormat { get; set; } = string.Empty;

    [JsonPropertyName("TerminPlatnosci")]
    public object PaymentTerm { get; set; } = new();

    [JsonPropertyName("SposobZaplaty")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("NazwaSeriiNumeracji")]
    public string NumberingSeriesName { get; set; } = string.Empty;

    [JsonPropertyName("NazwaSzablonu")]
    public string TemplateName { get; set; } = string.Empty;

    [JsonPropertyName("RodzajPodpisuOdbiorcy")]
    public string RecipientSignatureType { get; set; } = string.Empty;

    [JsonPropertyName("PodpisOdbiorcy")]
    public string RecipientSignature { get; set; } = string.Empty;

    [JsonPropertyName("PodpisWystawcy")]
    public string IssuerSignature { get; set; } = string.Empty;

    [JsonPropertyName("Uwagi")]
    public string Notes { get; set; } = string.Empty;

    [JsonPropertyName("WidocznyNumerGios")]
    public bool VisibleGiosNumber { get; set; }

    [JsonPropertyName("WidocznyNumerBdo")]
    public bool VisibleBdoNumber { get; set; }

    [JsonPropertyName("Numer")]
    public int? Number { get; set; }

    [JsonPropertyName("IdentyfikatorKontrahenta")]
    public string ContractorIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("PrefiksUEKontrahenta")]
    public string ContractorEuPrefix { get; set; } = string.Empty;

    [JsonPropertyName("NIPKontrahenta")]
    public string ContractorNip { get; set; } = string.Empty;

    [JsonPropertyName("Pozycje")]
    public InvoiceItem[] Items { get; set; } = Array.Empty<InvoiceItem>();

    [JsonPropertyName("Kontrahent")]
    public Contractor Contractor { get; set; } = new();
}
