using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class StatusResponseDto
{
    [JsonPropertyName("invoice_task_reference_number")]
    public string InvoiceTaskReferenceNumber { get; set; } = string.Empty;

    [JsonPropertyName("processing_code")]
    public int ProcessingCode { get; set; }

    [JsonPropertyName("processing_description")]
    public string ProcessingDescription { get; set; } = string.Empty;

    [JsonPropertyName("invoice_errors")]
    public InvoiceErrors InvoiceErrors { get; set; } = new();

    [JsonPropertyName("invoice")]
    public InvoiceResponseDto Invoice { get; set; } = new();
}
