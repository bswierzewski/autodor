using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.DTOs;

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

public class Contractor
{
    [JsonPropertyName("Nazwa")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("Nazwa2")]
    public string Name2 { get; set; } = string.Empty;
    
    [JsonPropertyName("Identyfikator")]
    public string Identifier { get; set; } = string.Empty;
    
    [JsonPropertyName("PrefiksUE")]
    public string EuPrefix { get; set; } = string.Empty;
    
    [JsonPropertyName("NIP")]
    public string Nip { get; set; } = string.Empty;
    
    [JsonPropertyName("Ulica")]
    public string Street { get; set; } = string.Empty;
    
    [JsonPropertyName("KodPocztowy")]
    public string PostalCode { get; set; } = string.Empty;
    
    [JsonPropertyName("Kraj")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("KodKraju")]
    public string CountryCode { get; set; } = string.Empty;
    
    [JsonPropertyName("Miejscowosc")]
    public string City { get; set; } = string.Empty;
    
    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("Telefon")]
    public string Phone { get; set; } = string.Empty;
    
    [JsonPropertyName("OsobaFizyczna")]
    public bool IsNaturalPerson { get; set; }
    
    [JsonPropertyName("JestOdbiorca")]
    public bool IsRecipient { get; set; }
    
    [JsonPropertyName("JestDostawca")]
    public bool IsSupplier { get; set; }
    
    [JsonPropertyName("PodmiotPowiazany")]
    public bool IsRelatedEntity { get; set; }
}

public class InvoiceItem
{
    [JsonPropertyName("StawkaVat")]
    public decimal VatRate { get; set; }
    
    [JsonPropertyName("Ilosc")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("CenaJednostkowa")]
    public float UnitPrice { get; set; }
    
    [JsonPropertyName("NazwaPelna")]
    public string FullName { get; set; } = string.Empty;
    
    [JsonPropertyName("Jednostka")]
    public string Unit { get; set; } = string.Empty;
    
    [JsonPropertyName("PKWiU")]
    public string PkwiuCode { get; set; } = string.Empty;
    
    [JsonPropertyName("GTU")]
    public string GtuCode { get; set; } = string.Empty;
    
    [JsonPropertyName("TypStawkiVat")]
    public string VatRateType { get; set; } = string.Empty;
    
    [JsonPropertyName("Rabat")]
    public int Discount { get; set; }
}

public class ResponseDto
{
    [JsonPropertyName("response")]
    public Response Response { get; set; } = new();
}

public class Response
{
    [JsonPropertyName("Kod")]
    public int Code { get; set; }

    [JsonPropertyName("Informacja")]
    public string Information { get; set; } = string.Empty;

    [JsonPropertyName("Identyfikator")]
    public string Identifier { get; set; } = string.Empty;
}