using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;

/// <summary>
/// Represents a contractor (customer/buyer) in iFirma API.
/// </summary>
public class Contractor
{
    /// <summary>
    /// Full name of the contractor's company.
    /// Required, 1-150 characters.
    /// </summary>
    [JsonPropertyName("Nazwa")]
    public required string Name { get; set; }

    /// <summary>
    /// Second name of the contractor's company (optional).
    /// Up to 150 characters.
    /// </summary>
    [JsonPropertyName("Nazwa2")]
    public string? Name2 { get; set; }

    /// <summary>
    /// Contractor identifier.
    /// Optional, up to 15 characters. If null, will be generated automatically.
    /// </summary>
    [JsonPropertyName("Identyfikator")]
    public string? Identifier { get; set; }

    /// <summary>
    /// EU prefix of the contractor (e.g., "PL").
    /// Optional, up to 2 characters.
    /// </summary>
    [JsonPropertyName("PrefiksUE")]
    public string? EUPrefix { get; set; }

    /// <summary>
    /// VAT number (NIP) of the contractor.
    /// Optional, up to 13 characters.
    /// </summary>
    [JsonPropertyName("NIP")]
    public string? VatNumber { get; set; }

    /// <summary>
    /// Street address of the contractor's headquarters.
    /// Optional, up to 65 characters.
    /// </summary>
    [JsonPropertyName("Ulica")]
    public string? Street { get; set; }

    /// <summary>
    /// Postal code of the contractor.
    /// Required, 1-16 characters.
    /// </summary>
    [JsonPropertyName("KodPocztowy")]
    public required string PostalCode { get; set; }

    /// <summary>
    /// Country of the contractor's headquarters.
    /// Optional, up to 70 characters.
    /// </summary>
    [JsonPropertyName("Kraj")]
    public string? Country { get; set; }

    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "PL").
    /// Optional, 2 characters. For Greece use "EL".
    /// </summary>
    [JsonPropertyName("KodKraju")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// City of the contractor's headquarters.
    /// Required, 1-65 characters.
    /// </summary>
    [JsonPropertyName("Miejscowosc")]
    public required string City { get; set; }

    /// <summary>
    /// Email address of the contractor.
    /// Optional, up to 65 characters.
    /// </summary>
    [JsonPropertyName("Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number of the contractor.
    /// Optional, up to 32 characters.
    /// </summary>
    [JsonPropertyName("Telefon")]
    public string? Phone { get; set; }

    /// <summary>
    /// Indicates if the contractor is a natural person.
    /// Optional, defaults to false.
    /// </summary>
    [JsonPropertyName("OsobaFizyczna")]
    public bool? IsNaturalPerson { get; set; }

    /// <summary>
    /// Indicates if the contractor is the recipient.
    /// Optional, defaults to false.
    /// </summary>
    [JsonPropertyName("JestOdbiorca")]
    public bool? IsRecipient { get; set; }

    /// <summary>
    /// Indicates if the contractor is the supplier.
    /// Optional, defaults to false.
    /// </summary>
    [JsonPropertyName("JestDostawca")]
    public bool? IsSupplier { get; set; }

    /// <summary>
    /// Indicates if the contractor is an associated entity with the seller.
    /// Optional, defaults to false.
    /// </summary>
    [JsonPropertyName("PodmiotPowiazany")]
    public bool? IsRelatedEntity { get; set; }
}
