using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;

/// <summary>
/// Response model from iFirma API after creating an invoice.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Status code returned by the API.
    /// 0 indicates success, non-zero values indicate errors.
    /// </summary>
    [JsonPropertyName("KodStatus")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Status message returned by the API.
    /// </summary>
    [JsonPropertyName("Wiadomosc")]
    public string? Message { get; set; }

    /// <summary>
    /// Unique identifier of the created invoice.
    /// Only present if invoice was created successfully.
    /// </summary>
    [JsonPropertyName("IdentyfikatorFaktury")]
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Full invoice number assigned by iFirma.
    /// Format: SERIES/NUMBER/YEAR
    /// Only present if invoice was created successfully.
    /// </summary>
    [JsonPropertyName("NumerPelny")]
    public string? FullInvoiceNumber { get; set; }

    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => StatusCode == 0;
}
