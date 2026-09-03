using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Extensions;
using DomainInvoice = Autodor.Modules.Invoicing.Domain.Aggregates.Invoice;

namespace Autodor.Tests.Integration.Invoicing.Clients;

public class IFirmaMappingTests
{
    [Fact]
    public void ToIFirmaInvoice_PreservesCalendarDatesAndAnnualNumberingSeries()
    {
        var invoice = new DomainInvoice
        {
            IssueDate = new DateOnly(2026, 9, 3),
            SaleDate = new DateOnly(2026, 9, 3),
            PaymentDue = new DateOnly(2026, 9, 17),
            Contractor = new Contractor("Test", "Leszno", "Testowa 1", "6961732144", "64-100", "test@example.com"),
            Items =
            [
                new InvoiceItem
                {
                    Name = "Usługa",
                    Quantity = 1,
                    UnitPrice = 100m
                }
            ]
        };

        var result = invoice.ToIFirmaInvoice();

        result.IssueDate.Should().Be(new DateOnly(2026, 9, 3));
        result.SalesDate.Should().Be(new DateOnly(2026, 9, 3));
        result.PaymentDeadline.Should().Be(new DateOnly(2026, 9, 17));
        result.NumberingSeriesName.Should().Be("Domyślna roczna");
    }
}
