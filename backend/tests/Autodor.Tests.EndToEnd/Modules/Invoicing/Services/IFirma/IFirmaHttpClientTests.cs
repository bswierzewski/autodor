using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Shared.Infrastructure.Tests.Core;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing.Services.IFirma;

/// <summary>
/// End-to-end tests for iFirma HTTP client with real API.
/// Requires iFirma sandbox credentials in environment variables:
/// </summary>
[Collection("Autodor")]
[Trait("Category", "Integration")]
public class IFirmaHttpClientTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private TestContext _context = null!;
    private IFirmaHttpClient _client => _context.GetRequiredService<IFirmaHttpClient>();

    public async Task InitializeAsync()
    {
        // Create test context with mocks injected
        _context = await TestContext.CreateBuilder<Program>()
            .WithContainer(shared.Container)
            .BuildAsync();

        await _context.ResetDatabaseAsync();
    }
    public async Task DisposeAsync()
    {
        if (_context != null)        
            await _context.DisposeAsync();        
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidInvoice_ShouldCreateAndReturnInvoice()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            Number = int.Parse(DateTime.Now.ToString("MMddHHmmss")),
            IssueDate = today,
            SalesDate = today,
            PaymentDeadline = today.AddDays(14),
            IssuePlace = "Leszno",
            PaymentMethod = PaymentMethodEnum.PRZ,
            CalculationType = CalculationTypeEnum.BRT,
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
        result.Should().NotBeNull();
        result.Response.Should().NotBeNull();
        result.Response.Message.Should().Be("Faktura została pomyślnie dodana.", "API should return success message");
        result.Response.StatusCode.Should().Be(0, "API should return success status");
    }
}
