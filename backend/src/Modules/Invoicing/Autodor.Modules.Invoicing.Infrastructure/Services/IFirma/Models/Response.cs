using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;

public class Response
{
    [JsonPropertyName("Kod")]
    public int Code { get; set; }

    [JsonPropertyName("Informacja")]
    public string Information { get; set; } = string.Empty;

    [JsonPropertyName("Identyfikator")]
    public string Identifier { get; set; } = string.Empty;
}
