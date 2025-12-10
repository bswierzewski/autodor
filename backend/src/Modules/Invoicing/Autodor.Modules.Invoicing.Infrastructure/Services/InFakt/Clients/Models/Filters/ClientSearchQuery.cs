using System.Text.Json.Serialization;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;

/// <summary>
/// Search query model for filtering clients in InFakt API.
/// </summary>
public class ClientSearchQuery
{
    /// <summary>
    /// Filter parameters.
    /// </summary>
    [JsonPropertyName("q")]
    public ClientSearchFilter Filter { get; set; } = new();
}

/// <summary>
/// Filter parameters for client search.
/// </summary>
public class ClientSearchFilter
{
    /// <summary>
    /// Search by exact NIP match.
    /// </summary>
    [JsonPropertyName("nip_eq")]
    public string? NipEq { get; set; }

    /// <summary>
    /// Search by NIP containing value.
    /// </summary>
    [JsonPropertyName("nip_cont")]
    public string? NipCont { get; set; }

    /// <summary>
    /// Search by exact company name match.
    /// </summary>
    [JsonPropertyName("company_name_eq")]
    public string? CompanyNameEq { get; set; }

    /// <summary>
    /// Search by company name containing value.
    /// </summary>
    [JsonPropertyName("company_name_cont")]
    public string? CompanyNameCont { get; set; }

    /// <summary>
    /// Search by email address.
    /// </summary>
    [JsonPropertyName("email_eq")]
    public string? EmailEq { get; set; }

    /// <summary>
    /// Search by phone number.
    /// </summary>
    [JsonPropertyName("phone_number_eq")]
    public string? PhoneNumberEq { get; set; }
}
