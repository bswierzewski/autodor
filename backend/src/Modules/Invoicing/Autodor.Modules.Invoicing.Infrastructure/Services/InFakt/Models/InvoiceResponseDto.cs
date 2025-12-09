using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class InvoiceResponseDto
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("invoice_date")]
    public string InvoiceDate { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("gross_price")]
    public int GrossPrice { get; set; }
}
