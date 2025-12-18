using DomainInvoice = Autodor.Modules.Invoicing.Domain.ValueObjects.Invoice;
using DomainContractor = Autodor.Modules.Invoicing.Domain.ValueObjects.Contractor;
using DomainInvoiceItem = Autodor.Modules.Invoicing.Domain.ValueObjects.InvoiceItem;
using InFaktInvoice = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests.Invoice;
using InFaktClient = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests.Client;
using InFaktInvoiceItem = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests.InvoiceItem;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Services;

public static class InFaktMappingExtensions
{
    public static InFaktInvoice ToInFaktInvoice(this DomainInvoice invoice)
    {
        return new InFaktInvoice
        {
            Number = invoice.Number?.ToString(),
            InvoiceDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
            SaleDate = invoice.SaleDate.ToString("yyyy-MM-dd"),
            PaymentDate = invoice.PaymentDue.ToString("yyyy-MM-dd"),
            PaymentMethod = invoice.PaymentMethod,
            Notes = string.IsNullOrEmpty(invoice.Notes) ? null : invoice.Notes,
            Kind = "vat",
            Currency = "PLN",
            ClientTaxCode = invoice.Contractor.NIP,
            Services = invoice.Items.Select(item => item.ToInFaktInvoiceItem()).ToList()
        };
    }

    public static InFaktClient ToInFaktClient(this DomainContractor contractor)
    {
        return new InFaktClient
        {
            CompanyName = contractor.Name,
            Street = contractor.Street,
            City = contractor.City,
            PostalCode = contractor.ZipCode,
            Nip = contractor.NIP,
            Email = string.IsNullOrEmpty(contractor.Email) ? null : contractor.Email,
            Country = "PL",
            BusinessActivityKind = "other_business"
        };
    }

    public static InFaktInvoiceItem ToInFaktInvoiceItem(this DomainInvoiceItem item)
    {
        var unitNetPrice = (int)(item.UnitPrice * 100); // Convert to groszy
        var totalNetPrice = (int)(item.UnitPrice * item.Quantity * 100); // Convert to groszy

        return new InFaktInvoiceItem
        {
            Name = item.Name,
            Unit = item.Unit,
            Quantity = item.Quantity,
            UnitNetPrice = unitNetPrice,
            NetPrice = totalNetPrice,
            TaxSymbol = MapVatRateToTaxSymbol(item.VatRate, item.VatType),
            Discount = item.Discount > 0 ? (int)item.Discount : null,
            PKWiU = string.IsNullOrEmpty(item.PKWiU) ? null : item.PKWiU
        };
    }

    private static string MapVatRateToTaxSymbol(decimal vatRate, string vatType)
    {
        // If VAT type is exempt, return "ZW"
        if (vatType?.ToUpperInvariant() == "ZW")
            return "ZW";

        // Convert decimal rate to percentage string (e.g., 0.23 -> "23")
        var percentage = (int)(vatRate * 100);
        return percentage.ToString();
    }
}
