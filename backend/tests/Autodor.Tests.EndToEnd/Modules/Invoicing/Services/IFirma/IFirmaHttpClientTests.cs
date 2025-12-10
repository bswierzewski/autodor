using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Shared.Infrastructure.Tests.Core;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing.Services.IFirma;

/// <summary>
/// End-to-end tests for iFirma HTTP client with real API.
/// Requires iFirma sandbox credentials in environment variables:
/// - IFIRMA_SANDBOX_API_KEY: API key for iFirma sandbox
/// - IFIRMA_SANDBOX_BASE_URL: Base URL (optional, defaults to https://www.ifirma.pl)
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
        var invoice = new Invoice
        {
            Paid = 100.00m,
            PaidOnDocument = 100.00m,
            CalculationType = "sum",
            IssueDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SalesDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SalesDateFormat = "date",
            PaymentMethod = "transfer",
            RecipientSignatureType = "none",
            VisibleGiosNumber = false,
            Number = int.Parse(DateTime.Now.ToString("MMddHHmmss")),
            Items = new List<InvoiceItem>
            {
                new()
                {
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    Name = "Test Service",
                    Unit = "szt",
                    VatRateType = "vat_23"
                }
            },
            Contractor = new Contractor
            {
                Name = "Test Client",
                PostalCode = "00-001",
                City = "Warsaw"
            }
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(0, "API should return success status");
        result.InvoiceId.Should().NotBeNull();
        result.FullInvoiceNumber.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithInvalidContractor_ShouldReturnErrorStatus()
    {
        // Arrange
        var invoice = new Invoice
        {
            Paid = 100.00m,
            PaidOnDocument = 100.00m,
            CalculationType = "sum",
            IssueDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SalesDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SalesDateFormat = "date",
            PaymentMethod = "transfer",
            RecipientSignatureType = "none",
            VisibleGiosNumber = false,
            Number = int.Parse(DateTime.Now.ToString("MMddHHmmss")),
            Items = new List<InvoiceItem>
            {
                new()
                {
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    Name = "Test",
                    Unit = "szt",
                    VatRateType = "vat_23"
                }
            },
            Contractor = new Contractor
            {
                Name = "",  // Invalid: empty name
                PostalCode = "00-001",
                City = "Warsaw"
            }
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().NotBe(0, "API should return error status for invalid contractor");
    }
}
