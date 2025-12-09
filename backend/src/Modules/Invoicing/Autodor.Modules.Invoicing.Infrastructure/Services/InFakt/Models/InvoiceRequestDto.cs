using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class InvoiceRequestDto
{
    [JsonPropertyName("invoice")]
    public InvoiceDto Invoice { get; set; } = new();
}
