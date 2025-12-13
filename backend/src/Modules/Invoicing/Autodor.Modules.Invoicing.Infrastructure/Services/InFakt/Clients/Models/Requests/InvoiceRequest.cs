using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

/// <summary>
/// Wrapper request model for creating an invoice in InFakt API.
/// InFakt API requires invoice data to be wrapped in an "invoice" property.
/// </summary>
public class InvoiceRequest
{
    /// <summary>
    /// Invoice data to be created.
    /// </summary>
    [JsonPropertyName("invoice")]
    public required Invoice Invoice { get; set; }
}