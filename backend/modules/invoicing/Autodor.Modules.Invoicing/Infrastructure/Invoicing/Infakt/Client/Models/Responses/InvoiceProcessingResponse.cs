using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;

/// <summary>
/// Status returned by the asynchronous InFakt invoice API.
/// </summary>
public class InvoiceProcessingResponse
{
    [JsonPropertyName("invoice_task_reference_number")]
    public string? TaskReferenceNumber { get; set; }

    [JsonPropertyName("processing_code")]
    public int ProcessingCode { get; set; }

    [JsonPropertyName("processing_description")]
    public string? ProcessingDescription { get; set; }

    [JsonPropertyName("invoice_uuid")]
    public string? InvoiceUuid { get; set; }

    [JsonPropertyName("invoice_errors")]
    public Dictionary<string, string[]>? InvoiceErrors { get; set; }
}
