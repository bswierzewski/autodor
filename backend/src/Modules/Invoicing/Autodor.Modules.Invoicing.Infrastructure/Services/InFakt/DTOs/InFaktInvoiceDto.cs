using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;

public class InFaktInvoiceRequestDto
{
    [JsonPropertyName("invoice")]
    public InFaktInvoiceDto Invoice { get; set; }
}

public class InFaktInvoiceDto
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("bank_name")]
    public string BankName { get; set; }

    [JsonPropertyName("bank_account")]
    public string BankAccount { get; set; }

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; }

    [JsonPropertyName("client_tax_code")]
    public string ClientTaxCode { get; set; }

    [JsonPropertyName("services")]
    public InFaktServiceDto[] Services { get; set; }
}

public class InFaktServiceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("net_price")]
    public int NetPrice { get; set; }

    [JsonPropertyName("tax_symbol")]
    public int TaxSymbol { get; set; }
}

public class InFaktStatusResponseDto
{
    [JsonPropertyName("invoice_task_reference_number")]
    public string InvoiceTaskReferenceNumber { get; set; }

    [JsonPropertyName("processing_code")]
    public int ProcessingCode { get; set; }

    [JsonPropertyName("processing_description")]
    public string ProcessingDescription { get; set; }

    [JsonPropertyName("invoice_errors")]
    public InFaktInvoiceErrors InvoiceErrors { get; set; }
}

public class InFaktInvoiceErrors
{
    [JsonPropertyName("base")]
    public string[] Base { get; set; }
}

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

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("nip")]
    public string NIP { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
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
}