using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Features;

internal static class InvoiceFactory
{
    public static Invoice Create(
        int? number,
        DateOnly issueDate,
        DateOnly saleDate,
        ContractorDto contractor,
        IReadOnlyList<InvoiceItem> items)
    {
        var invoiceContractor = new Contractor(
            Name: contractor.Name,
            City: contractor.City,
            Street: contractor.Street,
            NIP: contractor.NIP,
            ZipCode: contractor.ZipCode,
            Email: contractor.Email
        );

        return new Invoice
        {
            Number = number,
            IssueDate = issueDate,
            SaleDate = saleDate,
            PaymentDue = issueDate.AddDays(14),
            Contractor = invoiceContractor,
            Items = items
        };
    }
}
