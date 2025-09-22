using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.DTOs;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;

public static class MappingExtensions
{
    public static InvoiceDto ToInvoiceDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Number = invoice.Number,
            IssueDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
            SaleDate = invoice.SaleDate.ToString("yyyy-MM-dd"),
            PlaceOfIssue = invoice.PlaceOfIssue,
            PaymentTerm = invoice.PaymentDue,
            PaymentMethod = invoice.PaymentMethod,
            Notes = invoice.Notes,
            Contractor = new DTOs.Contractor
            {
                Name = invoice.Contractor.Name,
                Nip = invoice.Contractor.NIP,
                Street = invoice.Contractor.Street,
                PostalCode = invoice.Contractor.ZipCode,
                City = invoice.Contractor.City,
                Country = "PL", // Default to Poland
                Email = invoice.Contractor.Email,
                Phone = "" // Not available in current Contractor model
            },
            Items = invoice.Items.Select(item => new DTOs.InvoiceItem
            {
                FullName = item.Name,
                Unit = item.Unit,
                Quantity = item.Quantity,
                UnitPrice = (float)item.UnitPrice,
                VatRate = item.VatRate,
                VatRateType = item.VatType,
                Discount = (int)item.Discount,
                PkwiuCode = item.PKWiU,
                GtuCode = item.GTU
            }).ToArray()
        };
    }
}