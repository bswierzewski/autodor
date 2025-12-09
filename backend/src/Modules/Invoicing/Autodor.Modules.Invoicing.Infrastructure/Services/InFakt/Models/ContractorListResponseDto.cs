using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;

public class ContractorListResponseDto
{
    [JsonPropertyName("entities")]
    public ContractorResponseDto[] Entities { get; set; } = Array.Empty<ContractorResponseDto>();
}
