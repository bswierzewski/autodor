using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Models;

public class Contractor
{
    [JsonPropertyName("Nazwa")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Nazwa2")]
    public string Name2 { get; set; } = string.Empty;

    [JsonPropertyName("Identyfikator")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("PrefiksUE")]
    public string EuPrefix { get; set; } = string.Empty;

    [JsonPropertyName("NIP")]
    public string Nip { get; set; } = string.Empty;

    [JsonPropertyName("Ulica")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("KodPocztowy")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("Kraj")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("KodKraju")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("Miejscowosc")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("Telefon")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("OsobaFizyczna")]
    public bool IsNaturalPerson { get; set; }

    [JsonPropertyName("JestOdbiorca")]
    public bool IsRecipient { get; set; }

    [JsonPropertyName("JestDostawca")]
    public bool IsSupplier { get; set; }

    [JsonPropertyName("PodmiotPowiazany")]
    public bool IsRelatedEntity { get; set; }
}
