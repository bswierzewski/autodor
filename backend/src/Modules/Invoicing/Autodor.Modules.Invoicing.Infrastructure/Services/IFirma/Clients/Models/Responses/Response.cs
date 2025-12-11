using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;

/// <summary>
/// Base response model from iFirma API containing status information and result.
/// </summary>
public class Response
{
    /// <summary>
    /// Status code returned by the API.
    /// 0 indicates success, non-zero values indicate errors.
    /// </summary>
    [JsonPropertyName("Kod")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Status message returned by the API describing the result.
    /// </summary>
    [JsonPropertyName("Informacja")]
    public string? Message { get; set; }

    /// <summary>
    /// Result value from the API operation.
    /// The type and meaning depends on the specific endpoint.
    /// </summary>
    [JsonPropertyName("Wynik")]
    public string? Result { get; set; }

    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => StatusCode == 0;
}
