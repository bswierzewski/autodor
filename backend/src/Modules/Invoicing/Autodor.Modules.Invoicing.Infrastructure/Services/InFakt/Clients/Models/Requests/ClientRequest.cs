using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

/// <summary>
/// Wrapper request model for creating a client in InFakt API.
/// InFakt API requires client data to be wrapped in a "client" property.
/// </summary>
public class ClientRequest
{
    /// <summary>
    /// Client data to be created.
    /// </summary>
    [JsonPropertyName("client")]
    public required Client Client { get; set; }
}
