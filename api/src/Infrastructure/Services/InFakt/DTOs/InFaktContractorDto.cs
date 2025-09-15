using System.Text.Json.Serialization;

namespace Infrastructure.Services.InFakt.DTOs;

public class InFaktContractorRequestDto
{
    [JsonPropertyName("client")]
    public InFaktContractorDto Client { get; set; }
}

public class InFaktContractorDto
{
    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; }

    [JsonPropertyName("street")]
    public string Street { get; set; }

    [JsonPropertyName("street_number")]
    public string StreetNumber { get; set; }

    [JsonPropertyName("flat_number")]
    public string FlatNumber { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("nip")]
    public string NIP { get; set; }

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("same_forward_address")]
    public bool? SameForwardAddress { get; set; }

    [JsonPropertyName("web_site")]
    public string WebSite { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; }

    [JsonPropertyName("receiver")]
    public string Receiver { get; set; }

    [JsonPropertyName("mailing_company_name")]
    public string MailingCompanyName { get; set; }

    [JsonPropertyName("mailing_street")]
    public string MailingStreet { get; set; }

    [JsonPropertyName("mailing_city")]
    public string MailingCity { get; set; }

    [JsonPropertyName("mailing_postal_code")]
    public string MailingPostalCode { get; set; }

    [JsonPropertyName("days_to_payment")]
    public int? DaysToPayment { get; set; }

    [JsonPropertyName("invoice_note")]
    public string InvoiceNote { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("business_activity_kind")]
    public string BusinessActivityKind { get; set; }
}

public class InFaktContractorResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; }

    [JsonPropertyName("nip")]
    public string NIP { get; set; }
}

public class InFaktContractorListResponseDto
{
    [JsonPropertyName("entities")]
    public InFaktContractorResponseDto[] Entities { get; set; }

    [JsonPropertyName("metadata")]
    public InFaktMetadataDto Metadata { get; set; }
}

public class InFaktMetadataDto
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}