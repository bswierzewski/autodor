using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;

/// <summary>
/// Represents a client/customer in InFakt API.
/// </summary>
public class Client
{
    /// <summary>
    /// Client ID (if exists in InFakt).
    /// Optional. If provided, other client fields are not needed.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Company name.
    /// Required if business_activity_kind is set to business types and not provided via ID.
    /// </summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Client street address.
    /// Optional.
    /// </summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>
    /// Building number.
    /// Optional.
    /// </summary>
    [JsonPropertyName("street_number")]
    public string? StreetNumber { get; set; }

    /// <summary>
    /// Apartment/flat number.
    /// Optional.
    /// </summary>
    [JsonPropertyName("flat_number")]
    public string? FlatNumber { get; set; }

    /// <summary>
    /// City.
    /// Optional.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// ISO Alpha-2 country code (e.g., "PL").
    /// Required.
    /// </summary>
    [JsonPropertyName("country")]
    public required string Country { get; set; }

    /// <summary>
    /// Postal code in format NN-NNN.
    /// Optional.
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// VAT number (NIP).
    /// Optional.
    /// </summary>
    [JsonPropertyName("nip")]
    public string? Nip { get; set; }

    /// <summary>
    /// Phone number.
    /// Optional.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Whether mailing address is the same as company address.
    /// Optional, defaults to true.
    /// </summary>
    [JsonPropertyName("same_forward_address")]
    public bool? SameForwardAddress { get; set; }

    /// <summary>
    /// Website URL.
    /// Optional.
    /// </summary>
    [JsonPropertyName("web_site")]
    public string? Website { get; set; }

    /// <summary>
    /// Email address.
    /// Optional.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Notes about the client.
    /// Optional.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Document receiver name.
    /// Optional.
    /// </summary>
    [JsonPropertyName("receiver")]
    public string? Receiver { get; set; }

    /// <summary>
    /// Company name for mailing.
    /// Optional.
    /// </summary>
    [JsonPropertyName("mailing_company_name")]
    public string? MailingCompanyName { get; set; }

    /// <summary>
    /// Street for mailing.
    /// Optional.
    /// </summary>
    [JsonPropertyName("mailing_street")]
    public string? MailingStreet { get; set; }

    /// <summary>
    /// City for mailing.
    /// Optional.
    /// </summary>
    [JsonPropertyName("mailing_city")]
    public string? MailingCity { get; set; }

    /// <summary>
    /// Postal code for mailing in format NN-NNN.
    /// Optional.
    /// </summary>
    [JsonPropertyName("mailing_postal_code")]
    public string? MailingPostalCode { get; set; }

    /// <summary>
    /// Default payment deadline in days for this client.
    /// Optional.
    /// </summary>
    [JsonPropertyName("days_to_payment")]
    public int? DaysToPayment { get; set; }

    /// <summary>
    /// Default invoice notes for this client.
    /// Optional.
    /// </summary>
    [JsonPropertyName("invoice_note")]
    public string? InvoiceNote { get; set; }

    /// <summary>
    /// Default payment method.
    /// Optional. Values: transfer, cash, card, barter, check, bill_of_sale, delivery, compensation, accredited, paypal, instalment_sale, payu, tpay, przelewy24, dotpay, other.
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Client's first name.
    /// Required if business_activity_kind indicates private person or self-employed and not provided via ID.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Client's last name.
    /// Required if business_activity_kind indicates private person or self-employed and not provided via ID.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Type of business activity.
    /// Optional. Values: private_person, self_employed, other_business.
    /// </summary>
    [JsonPropertyName("business_activity_kind")]
    public string? BusinessActivityKind { get; set; }
}
