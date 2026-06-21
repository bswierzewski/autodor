using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Requests;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Invoicing.Clients;

[Collection(SharedCollection.Name)]
public class IFirmaHttpClientTests(AutodorDatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{

    [Fact(Skip = "Manual test - requires real IFirma API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithValidInvoice_ShouldCreateAndReturnInvoice()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            IssueDate = today,
            SalesDate = today,
            PaymentDeadline = today.AddDays(14),
            IssuePlace = "Leszno",
            PaymentMethod = PaymentMethodEnum.PRZ,
            CalculationType = CalculationTypeEnum.NET,
            SalesDateFormat = SalesDateFormatEnum.DZN,
            RecipientSignatureType = RecipientSignatureTypeEnum.OUP,
            NumberingSeriesName = "Domyślna roczna",
            Paid = 0,
            PaidOnDocument = 0,
            VisibleGiosNumber = false,
            Contractor = new Contractor
            {
                VatNumber = "1549402951",
                Name = "Test Client",
                Street = "Test Street 1",
                PostalCode = "00-001",
                City = "Warsaw",
                Country = "Polska",
                CountryCode = "PL",
                Email = "test@test.com",
                Phone = "123456789",
                IsRecipient = true
            },
            Items =
            [
                new()
                {
                    Name = "Test Service",
                    Unit = "szt",
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    VatRate = null,
                    VatRateType = VatRateTypeEnum.ZW,
                }
            ]
        };

        // Act
        await using var scope = Host.Services.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<IIFirmaHttpClient>();
        var result = await client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Response.Message.Should().Be("Faktura została pomyślnie dodana.", "API should return success message");
        result.Response.Should().NotBeNull();
        result.Response.StatusCode.Should().Be(0, "API should return success status");
        result.Response.IsSuccess.Should().BeTrue();
    }

    [Fact(Skip = "Manual test - requires real IFirma API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithVatRate_ShouldCreateAndReturnInvoice()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            IssueDate = today,
            SalesDate = today,
            PaymentDeadline = today.AddDays(14),
            IssuePlace = "Leszno",
            PaymentMethod = PaymentMethodEnum.PRZ,
            CalculationType = CalculationTypeEnum.NET,
            SalesDateFormat = SalesDateFormatEnum.DZN,
            RecipientSignatureType = RecipientSignatureTypeEnum.OUP,
            NumberingSeriesName = "Domyślna roczna",
            Paid = 0,
            PaidOnDocument = 0,
            VisibleGiosNumber = false,
            Contractor = new Contractor
            {
                VatNumber = "1549402951",
                Name = "Test Client VAT",
                Street = "Test Street 2",
                PostalCode = "00-002",
                City = "Warsaw",
                Country = "Polska",
                CountryCode = "PL",
                Email = "test-vat@test.com",
                IsRecipient = true
            },
            Items =
            [
                new()
                {
                    Name = "Test Service with VAT",
                    Unit = "szt",
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    VatRate = 0.23m,
                    VatRateType = VatRateTypeEnum.PRC,
                    PKWiUCode = "62.01.11.0"
                }
            ]
        };

        // Act
        await using var scope = Host.Services.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<IIFirmaHttpClient>();
        var result = await client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Response.IsSuccess.Should().BeTrue();
        result.Response.StatusCode.Should().Be(0);
    }

    [Fact(Skip = "Manual test - requires real IFirma API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithMissingRequiredField_ShouldReturnError()
    {
        // Arrange - Missing required Contractor field
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            IssueDate = today,
            SalesDate = today,
            PaymentDeadline = today.AddDays(14),
            IssuePlace = "Leszno",
            PaymentMethod = PaymentMethodEnum.PRZ,
            CalculationType = CalculationTypeEnum.NET,
            SalesDateFormat = SalesDateFormatEnum.DZN,
            RecipientSignatureType = RecipientSignatureTypeEnum.OUP,
            NumberingSeriesName = "Domyślna roczna",
            Paid = 0,
            PaidOnDocument = 0,
            VisibleGiosNumber = false,
            Contractor = null!,  // Intentionally null to trigger API error
            Items =
            [
                new()
                {
                    Name = "Test Service",
                    Unit = "szt",
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    VatRate = 0.23M,
                    VatRateType = VatRateTypeEnum.PRC,
                }
            ]
        };

        // Act
        await using var scope = Host.Services.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<IIFirmaHttpClient>();
        var action = () => client.CreateInvoiceAsync(invoice);

        // Assert
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact(Skip = "Manual test - requires real IFirma API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithInvalidVatNumber_ShouldReturnError()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            IssueDate = today,
            SalesDate = today,
            PaymentDeadline = today.AddDays(14),
            IssuePlace = "Leszno",
            PaymentMethod = PaymentMethodEnum.PRZ,
            CalculationType = CalculationTypeEnum.NET,
            SalesDateFormat = SalesDateFormatEnum.DZN,
            RecipientSignatureType = RecipientSignatureTypeEnum.OUP,
            NumberingSeriesName = "Domyślna roczna",
            Paid = 0,
            PaidOnDocument = 0,
            VisibleGiosNumber = false,
            Contractor = new Contractor
            {
                VatNumber = "INVALID", // Invalid NIP
                Name = "Test Client",
                Street = "Test Street",
                PostalCode = "00-001",
                City = "Warsaw",
                Country = "Polska",
                CountryCode = "PL",
                IsRecipient = true
            },
            Items =
            [
                new()
                {
                    Name = "Test Service",
                    Unit = "szt",
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    VatRate = 0.23m,
                    VatRateType = VatRateTypeEnum.PRC,
                }
            ]
        };

        // Act
        await using var scope = Host.Services.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<IIFirmaHttpClient>();
        var action = () => client.CreateInvoiceAsync(invoice);

        // Assert
        await action.Should().ThrowAsync<Exception>();
    }
}
