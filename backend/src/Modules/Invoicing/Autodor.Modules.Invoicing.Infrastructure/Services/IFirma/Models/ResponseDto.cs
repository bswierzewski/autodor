using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;

public class ResponseDto
{
    [JsonPropertyName("response")]
    public Response Response { get; set; } = new();
}
