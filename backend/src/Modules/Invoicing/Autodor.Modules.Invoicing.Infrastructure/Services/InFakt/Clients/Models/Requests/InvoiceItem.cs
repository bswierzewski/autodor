using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

/// <summary>
/// Represents a service/product line item on an InFakt invoice.
/// </summary>
public class InvoiceItem
{
    /// <summary>
    /// Name of the item/service.
    /// Required.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// VAT rate symbol (tax rate).
    /// Optional. Available rates: 0%, 5%, 8%, 23%, ZW (exempt), etc.
    /// </summary>
    [JsonPropertyName("tax_symbol")]
    public string? TaxSymbol { get; set; }

    /// <summary>
    /// Unit of measurement (e.g., "szt", "kg", "m").
    /// Optional.
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>
    /// Quantity of items.
    /// Optional.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>
    /// Net unit price in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("unit_net_price")]
    public int? UnitNetPrice { get; set; }

    /// <summary>
    /// Total net price in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("net_price")]
    public int? NetPrice { get; set; }

    /// <summary>
    /// Total gross price in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("gross_price")]
    public int? GrossPrice { get; set; }

    /// <summary>
    /// VAT tax amount in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("tax_price")]
    public int? TaxPrice { get; set; }

    /// <summary>
    /// PKWiU code (product classification).
    /// Optional.
    /// </summary>
    [JsonPropertyName("pkwiu")]
    public string? PKWiU { get; set; }

    /// <summary>
    /// CN code (Combined Nomenclature).
    /// Optional.
    /// </summary>
    [JsonPropertyName("cn")]
    public string? CN { get; set; }

    /// <summary>
    /// PKOB code.
    /// Optional.
    /// </summary>
    [JsonPropertyName("pkob")]
    public string? PKOB { get; set; }

    /// <summary>
    /// Discount percentage.
    /// Optional, value between 0-100.
    /// </summary>
    [JsonPropertyName("discount")]
    public int? Discount { get; set; }

    /// <summary>
    /// Net unit price before discount in groszy (cents).
    /// Optional.
    /// </summary>
    [JsonPropertyName("unit_net_price_before_discount")]
    public int? UnitNetPriceBeforeDiscount { get; set; }

    /// <summary>
    /// GTU code ID (Goods/Services Group).
    /// Optional.
    /// </summary>
    [JsonPropertyName("gtu_id")]
    public int? GTUId { get; set; }

    /// <summary>
    /// VAT date value preference.
    /// Optional. Values: issue_date, sale_date, paid_date.
    /// </summary>
    [JsonPropertyName("vat_date_value")]
    public string? VatDateValue { get; set; }

    /// <summary>
    /// Occasional sale indicator.
    /// Optional.
    /// </summary>
    [JsonPropertyName("occasional_sale")]
    public bool? OccasionalSale { get; set; }

    /// <summary>
    /// Flat rate tax symbol (for flat rate taxation).
    /// Optional. Example: "8.5".
    /// </summary>
    [JsonPropertyName("flat_rate_tax_symbol")]
    public string? FlatRateTaxSymbol { get; set; }
}
