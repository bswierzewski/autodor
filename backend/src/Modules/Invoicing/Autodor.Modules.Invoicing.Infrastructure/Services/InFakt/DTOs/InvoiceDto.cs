using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;

public class InvoiceRequestDto
{
    [JsonPropertyName("invoice")]
    public InvoiceDto Invoice { get; set; } = new();
}

public class InvoiceDto
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("bank_name")]
    public string BankName { get; set; } = string.Empty;

    [JsonPropertyName("bank_account")]
    public string BankAccount { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("client_tax_code")]
    public string ClientTaxCode { get; set; } = string.Empty;

    [JsonPropertyName("services")]
    public ServiceDto[] Services { get; set; } = Array.Empty<ServiceDto>();
}

public class ServiceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("net_price")]
    public int NetPrice { get; set; }

    [JsonPropertyName("tax_symbol")]
    public int TaxSymbol { get; set; }
}

public class StatusResponseDto
{
    [JsonPropertyName("invoice_task_reference_number")]
    public string InvoiceTaskReferenceNumber { get; set; } = string.Empty;

    [JsonPropertyName("processing_code")]
    public int ProcessingCode { get; set; }

    [JsonPropertyName("processing_description")]
    public string ProcessingDescription { get; set; } = string.Empty;

    [JsonPropertyName("invoice_errors")]
    public InvoiceErrors InvoiceErrors { get; set; } = new();

    [JsonPropertyName("invoice")]
    public InvoiceResponseDto Invoice { get; set; } = new();
}

public class InvoiceResponseDto
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("invoice_date")]
    public string InvoiceDate { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("gross_price")]
    public int GrossPrice { get; set; }
}

public class InvoiceErrors
{
    [JsonPropertyName("base")]
    public string[] Base { get; set; } = Array.Empty<string>();
}

public class ContractorRequestDto
{
    [JsonPropertyName("client")]
    public ContractorDto Client { get; set; } = new();
}

public class ContractorDto
{
    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("nip")]
    public string NIP { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class ContractorResponseDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("nip")]
    public string NIP { get; set; } = string.Empty;
}

public class ContractorListResponseDto
{
    [JsonPropertyName("entities")]
    public ContractorResponseDto[] Entities { get; set; } = Array.Empty<ContractorResponseDto>();
}