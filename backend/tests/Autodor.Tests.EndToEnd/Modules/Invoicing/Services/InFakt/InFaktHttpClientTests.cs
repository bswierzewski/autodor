using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using Shared.Infrastructure.Tests.Core;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing.Services.InFakt;

/// <summary>
/// End-to-end tests for InFakt HTTP client with real API.
/// Requires InFakt sandbox credentials in environment variables:
/// - INFAKT_SANDBOX_API_KEY: API key for InFakt sandbox
/// - INFAKT_SANDBOX_BASE_URL: Base URL (optional, defaults to sandbox)
/// </summary>
[Collection("Autodor")]
[Trait("Category", "Integration")]
public class InFaktHttpClientTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private TestContext _context = null!;
    private InFaktHttpClient _client => _context.GetRequiredService<InFaktHttpClient>();

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
    public async Task GetClientsAsync_WithEmptyFilter_ShouldReturnClientList()
    {
        // Arrange
        var query = new ClientSearchQuery();

        // Act
        var result = await _client.GetClientsAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Entities.Should().NotBeNull();
    }

    [Fact]
    public async Task GetClientsAsync_WithNipFilter_ShouldReturnMatchingClients()
    {
        // Arrange - Using a valid test NIP
        var query = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter { NipEq = "1176224556" }
        };

        // Act
        var result = await _client.GetClientsAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Entities.Should().NotBeNull();
        // Filter may return 0 results if client doesn't exist in sandbox
    }

    [Fact]
    public async Task GetClientAsync_WithValidId_ShouldReturnClient()
    {
        // Arrange - First create a test client
        var newClient = new Client
        {
            Country = "PL",
            FirstName = "GetTest",
            LastName = "User",
            BusinessActivityKind = "private_person",
            Email = $"get-test-{Guid.NewGuid()}@example.com"
        };

        var createdClient = await _client.CreateClientAsync(newClient);
        createdClient.Id.Should().NotBeNull();

        // Act
        var retrievedClient = await _client.GetClientAsync(createdClient.Id!.Value);

        // Assert
        retrievedClient.Should().NotBeNull();
        retrievedClient.FirstName.Should().Be("GetTest");
        retrievedClient.Id.Should().Be(createdClient.Id);
    }

    [Fact]
    public async Task CreateClientAsync_WithValidClient_ShouldCreateAndReturnClient()
    {
        // Arrange
        var client = new Client
        {
            BusinessActivityKind = "self_employed",
            Nip = "1176224556",  // Valid 10-digit NIP from documentation
            FirstName = "Test",
            LastName = "User",
            CompanyName = "Test Company Test User",  // Required for self_employed
            Street = "Testowa",
            StreetNumber = "1",
            City = "Warsaw",
            Country = "PL",
            PostalCode = "00-001"
        };

        // Act
        var result = await _client.CreateClientAsync(client);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("User");
        result.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidInvoice_ShouldCreateInvoice()
    {
        var invoice = new Invoice
        {
            Currency = "PLN",
            Notes = "Test invoice from API",
            Kind = "vat",
            PaymentMethod = "transfer",
            ClientTaxCode = "1176224556", // Known valid NIP from creation
            InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SaleDate = DateTime.Now.ToString("yyyy-MM-dd"),
            Services = new List<InvoiceItem>
            {
                new()
                {
                    Name = "Test Service",
                    TaxSymbol = "23",
                    Unit = "szt",
                    Quantity = 1,
                    UnitNetPrice = 10000 // 100.00 PLN in groszy
                }
            }
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNull();
        result.Status.Should().BeOneOf("draft", "sent", "printed", "paid");
    }
}
