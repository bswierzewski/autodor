using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class InvoiceDto
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("bank_name")]
    public string BankName { get; set; } = string.Empty;

    [JsonPropertyName("bank_account")]
    public string BankAccount { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("client_tax_code")]
    public string ClientTaxCode { get; set; } = string.Empty;

    [JsonPropertyName("services")]
    public ServiceDto[] Services { get; set; } = Array.Empty<ServiceDto>();
}
