using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;

/// <summary>
/// Represents a line item on an invoice in iFirma API.
/// </summary>
public class InvoiceItem
{
    /// <summary>
    /// VAT rate for this item.
    /// Required. Valid values: null, 0.00, 0.05, 0.08, 0.23.
    /// </summary>
    [JsonPropertyName("StawkaVat")]
    public decimal? VatRate { get; set; }

    /// <summary>
    /// Quantity of goods or services.
    /// Required, must be > 0.0000 and <= 12 digits.
    /// </summary>
    [JsonPropertyName("Ilosc")]
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Unit price of goods or services.
    /// Required, must be > 0.00 and < 100000000.
    /// </summary>
    [JsonPropertyName("CenaJednostkowa")]
    public required decimal UnitPrice { get; set; }

    /// <summary>
    /// Full name of goods or services.
    /// Required, 1-300 characters.
    /// </summary>
    [JsonPropertyName("NazwaPelna")]
    public required string Name { get; set; }

    /// <summary>
    /// Unit of measurement (e.g., "szt", "kg", "m").
    /// Required, 1-10 characters.
    /// </summary>
    [JsonPropertyName("Jednostka")]
    public required string Unit { get; set; }

    /// <summary>
    /// PKWiU code (product classification code).
    /// Optional, up to 30 characters.
    /// Required if TypStawkiVat != ZW (not exempt).
    /// </summary>
    [JsonPropertyName("PKWiU")]
    public string? PKWiUCode { get; set; }

    /// <summary>
    /// GTU symbol (goods/services group).
    /// Optional. Valid values: BRAK, 01-13.
    /// </summary>
    [JsonPropertyName("GTU")]
    public string? GTU { get; set; }

    /// <summary>
    /// Type of VAT rate.
    /// Required. Values: PRC (percentage), ZW (exempt).
    /// </summary>
    [JsonPropertyName("TypStawkiVat")]
    public required string VatRateType { get; set; }

    /// <summary>
    /// Discount percentage.
    /// Optional, >= 0 and < 100.
    /// </summary>
    [JsonPropertyName("Rabat")]
    public decimal? Discount { get; set; }
}
