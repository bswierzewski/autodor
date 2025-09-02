using System.Text.Json.Serialization;

namespace Infrastructure.Services.InFakt.DTOs;

public class InFaktStatusResponseDto
{
    [JsonPropertyName("timestamps")]
    public InFaktTimestampsDto Timestamps { get; set; }

    [JsonPropertyName("invoice_task_reference_number")]
    public string InvoiceTaskReferenceNumber { get; set; }

    [JsonPropertyName("processing_code")]
    public int ProcessingCode { get; set; }

    [JsonPropertyName("processing_description")]
    public string ProcessingDescription { get; set; }

    [JsonPropertyName("invoice_uuid")]
    public string InvoiceUuid { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("invoice_kind")]
    public string InvoiceKind { get; set; }

    [JsonPropertyName("invoice_errors")]
    public InFaktInvoiceErrorsDto InvoiceErrors { get; set; }
}

public class InFaktTimestampsDto
{
    [JsonPropertyName("task_created_at")]
    public string TaskCreatedAt { get; set; }
}

public class InFaktInvoiceErrorsDto
{
    [JsonPropertyName("base")]
    public string[] Base { get; set; }
}