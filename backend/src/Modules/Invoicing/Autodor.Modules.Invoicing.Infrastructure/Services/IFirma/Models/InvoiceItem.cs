using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;

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
