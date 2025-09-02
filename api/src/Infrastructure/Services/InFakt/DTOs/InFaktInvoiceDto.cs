using System.Text.Json.Serialization;

namespace Infrastructure.Services.InFakt.DTOs;

public class InFaktInvoiceRequestDto
{
    [JsonPropertyName("invoice")]
    public InFaktInvoiceDto Invoice { get; set; }
}

public class InFaktInvoiceDto
{
    [JsonPropertyName("bank_account")]
    public string BankAccount { get; set; }

    [JsonPropertyName("bank_name")]
    public string BankName { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; }

    [JsonPropertyName("client_company_name")]
    public string ClientCompanyName { get; set; }

    [JsonPropertyName("client_business_activity_kind")]
    public string ClientBusinessActivityKind { get; set; }

    [JsonPropertyName("client_street")]
    public string ClientStreet { get; set; }

    [JsonPropertyName("client_street_number")]
    public string ClientStreetNumber { get; set; }

    [JsonPropertyName("client_city")]
    public string ClientCity { get; set; }

    [JsonPropertyName("client_post_code")]
    public string ClientPostCode { get; set; }

    [JsonPropertyName("client_tax_code")]
    public string ClientTaxCode { get; set; }

    [JsonPropertyName("client_country")]
    public string ClientCountry { get; set; }

    [JsonPropertyName("services")]
    public InFaktServiceDto[] Services { get; set; }
}

public class InFaktServiceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("net_price")]
    public int NetPrice { get; set; }

    [JsonPropertyName("gross_price")]
    public int GrossPrice { get; set; }

    [JsonPropertyName("unit_net_price")]
    public int UnitNetPrice { get; set; }

    [JsonPropertyName("tax_symbol")]
    public int TaxSymbol { get; set; }
}