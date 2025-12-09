using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class InvoiceErrors
{
    [JsonPropertyName("base")]
    public string[] Base { get; set; } = Array.Empty<string>();
}
