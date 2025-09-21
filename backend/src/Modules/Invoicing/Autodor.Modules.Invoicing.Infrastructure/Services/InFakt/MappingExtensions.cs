using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.DTOs;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public static class MappingExtensions
{
    public static InFaktInvoiceRequestDto ToInvoiceDto(this Invoice invoice)
    {
        return new InFaktInvoiceRequestDto
        {
            Invoice = new InFaktInvoiceDto
            {
                PaymentMethod = "transfer",
                BankName = "PKO BANK POLSKI",
                BankAccount = "56102030880000820200920322",
                PaymentDate = invoice.PaymentDue.ToString("yyyy-MM-dd"),
                ClientTaxCode = invoice.Contractor.NIP,
                Services = invoice.Items.Select(item => new InFaktServiceDto
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    NetPrice = (int)(item.UnitPrice * item.Quantity * 100), // konwersja na grosze
                    TaxSymbol = (int)(item.VatRate * 100) // konwersja np. 0.23 -> 23
                }).ToArray()
            }
        };
    }

    public static InFaktContractorRequestDto ToContractorDto(this Contractor contractor)
    {
        return new InFaktContractorRequestDto
        {
            Client = new InFaktContractorDto
            {
                CompanyName = contractor.Name,
                Street = contractor.Street,
                City = contractor.City,
                PostalCode = contractor.ZipCode,
                NIP = contractor.NIP,
                Country = "PL",
                Email = contractor.Email
            }
        };
    }
}