using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;

/// <summary>
/// Envelope wrapping iFirma API responses.
/// </summary>
public class Envelope
{
    /// <summary>
    /// Response data containing status and result information.
    /// </summary>
    [JsonPropertyName("response")]
    public Response? Response { get; set; }

    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Response?.IsSuccess ?? false;
}
