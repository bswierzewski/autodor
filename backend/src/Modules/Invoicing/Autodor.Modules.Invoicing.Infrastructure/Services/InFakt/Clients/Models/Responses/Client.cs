using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Responses;

/// <summary>
/// Response model for a single client from InFakt API.
/// </summary>
public class Client
{
    /// <summary>
    /// Client ID assigned by InFakt.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Company name.
    /// </summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Client's first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Client's last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// VAT number (NIP).
    /// </summary>
    [JsonPropertyName("nip")]
    public string? Nip { get; set; }

    /// <summary>
    /// Client street address.
    /// </summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>
    /// Building number.
    /// </summary>
    [JsonPropertyName("street_number")]
    public string? StreetNumber { get; set; }

    /// <summary>
    /// Apartment/flat number.
    /// </summary>
    [JsonPropertyName("flat_number")]
    public string? FlatNumber { get; set; }

    /// <summary>
    /// City.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// ISO Alpha-2 country code (e.g., "PL").
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Postal code.
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Type of business activity.
    /// Values: private_person, self_employed, other_business.
    /// </summary>
    [JsonPropertyName("business_activity_kind")]
    public string? BusinessActivityKind { get; set; }
}
