using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using ErrorOr;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing.Services.IFirma;

/// <summary>
/// End-to-end tests for iFirma HTTP client with real API.
/// Requires iFirma sandbox credentials in environment variables:
/// </summary>
[Collection("Autodor")]
[Trait("Category", "Integration")]
public class IFirmaHttpClientTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{

    private IFirmaHttpClient _client => Context.GetRequiredService<IFirmaHttpClient>();

    [Fact]
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
            Contractor = new Contractor
            {
                VatNumber = "1549402951",
                Name = "Test Client",
                Street = "Test Street 1",
                PostalCode = "00-001",
                City = "Warsaw",
                Country = "PL",
                Email = "test@test.com",
                Phone = "123456789"
            },
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
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Response.Should().NotBeNull();
        result.Value.Response.Message.Should().Be("Faktura została pomyślnie dodana.", "API should return success message");
        result.Value.Response.StatusCode.Should().Be(0, "API should return success status");
    }

    [Fact]
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
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.IsError.Should().BeTrue("API should return error for missing required field");
        result.Errors.Should().NotBeEmpty();
        result.FirstError.Code.Should().Be("IFirma.BusinessValidationError");
        result.FirstError.Type.Should().Be(ErrorType.Validation, "Should return validation error with detailed business messages");
        result.FirstError.Description.Should().NotBeEmpty("Should contain detailed error message from API");
    }
}
