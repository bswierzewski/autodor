using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class ContractorRequestDto
{
    [JsonPropertyName("client")]
    public ContractorDto Client { get; set; } = new();
}
