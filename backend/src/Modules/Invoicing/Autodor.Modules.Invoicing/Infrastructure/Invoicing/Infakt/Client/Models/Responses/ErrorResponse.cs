using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? Errors { get; set; }
}
