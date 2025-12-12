using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;
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
        var invoice = new Invoice
        {
            Number = int.Parse(DateTime.Now.ToString("MMddHHmmss")),
            IssueDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SalesDate = DateTime.Now.ToString("yyyy-MM-dd"),
            PaymentDeadline = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"),
            PaymentMethod = "PRZ",
            CalculationType = "BRT",
            Contractor = new Contractor
            {
                Name = "Test Client",
                VatNumber = "123",
                Street = "Test Street 1",
                PostalCode = "00-001",
                City = "Warsaw",
                Country = "PL",
                Email = "",
                Phone = ""
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
                    VatRateType = "PRC",
                }
            ]
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Response.Should().NotBeNull();
        result.Response.StatusCode.Should().Be(0, "API should return success status");
        result.Response.Message.Should().Be("Faktura została pomyślnie dodana.", "API should return success message");
    }
}
