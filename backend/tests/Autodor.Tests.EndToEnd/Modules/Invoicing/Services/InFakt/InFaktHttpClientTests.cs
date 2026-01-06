using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using ErrorOr;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing.Services.InFakt;

/// <summary>
/// End-to-end tests for InFakt HTTP client with real API.
/// Requires InFakt sandbox credentials in environment variables:
/// - INFAKT_SANDBOX_API_KEY: API key for InFakt sandbox
/// - INFAKT_SANDBOX_BASE_URL: Base URL (optional, defaults to sandbox)
/// </summary>
[Collection("Autodor")]
[Trait("Category", "Integration")]
public class InFaktHttpClientTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{

    private InFaktHttpClient _client => Context.GetRequiredService<InFaktHttpClient>();

    [Fact]
    public async Task GetClientsAsync_WithEmptyFilter_ShouldReturnClientList()
    {
        // Arrange
        var query = new ClientSearchQuery();

        // Act
        var result = await _client.GetClientsAsync(query);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Entities.Should().NotBeNull();
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
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Entities.Should().NotBeNull();
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

        var createdClientResult = await _client.CreateClientAsync(newClient);
        createdClientResult.IsError.Should().BeFalse();
        createdClientResult.Value.Id.Should().NotBeNull();

        // Act
        var result = await _client.GetClientAsync(createdClientResult.Value.Id!.Value);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be("GetTest");
        result.Value.Id.Should().Be(createdClientResult.Value.Id);
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
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be("Test");
        result.Value.LastName.Should().Be("User");
        result.Value.Id.Should().NotBeNull();
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
        result.Value.Id.Should().NotBeNull();
        result.Value.Status.Should().BeOneOf("draft", "sent", "printed", "paid");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithInvalidInvoice_ShouldReturnError()
    {
        var invoice = new Invoice
        {
            Currency = "PLN",
            Notes = "Test invoice from API",
            Kind = "vats",
            PaymentMethod = "transfera",
            ClientTaxCode = "117622455622", // Known valid NIP from creation
            InvoiceDate = DateTime.Now.ToString("yyyy-dd-MM"),
            SaleDate = DateTime.Now.ToString("yyyy-MM-dd"),
            Services = new List<InvoiceItem>
            {
                new()
                {
                    Name = "Test Service",
                    TaxSymbol = "231",
                    Unit = "szta",
                    Quantity = -1,
                    UnitNetPrice = 10000 // 100.00 PLN in groszy
                }
            }
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.IsError.Should().BeTrue("API should return error for missing required field");
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithMissingRequiredField_ShouldReturnError()
    {
        // Arrange - Missing required Services field
        var invoice = new Invoice
        {
            Currency = "PLN",
            Notes = "Test invoice with error",
            Kind = "vat",
            PaymentMethod = "transfer",
            ClientTaxCode = "1176224556",
            InvoiceDate = DateTime.Now.ToString("yyyy-MM-dd"),
            SaleDate = DateTime.Now.ToString("yyyy-MM-dd"),
            Services = null!  // Intentionally null to trigger API error
        };

        // Act
        var result = await _client.CreateInvoiceAsync(invoice);

        // Assert
        result.IsError.Should().BeTrue("API should return error for missing required field");
        result.Errors.Should().NotBeEmpty();
        result.FirstError.Code.Should().Be("InFakt.CreateInvoiceFailed.services", "Error code should include the field name");
        result.FirstError.Type.Should().Be(ErrorType.Validation, "Should return validation error with detailed business messages");
        result.FirstError.Description.Should().Be("Proszę dodać pozycję na fakturze.");
    }

    [Fact]
    public async Task GetClientAsync_WithInvalidId_ShouldReturnError()
    {
        // Arrange - Using a non-existent client ID
        var invalidClientId = 999999999;

        // Act
        var result = await _client.GetClientAsync(invalidClientId);

        // Assert
        result.IsError.Should().BeTrue("API should return error for non-existent client");
        result.Errors.Should().NotBeEmpty();
        result.FirstError.Code.Should().Be("InFakt.GetClientFailed");
        result.FirstError.Description.Should().NotBeEmpty("Zasób którego szukasz nie został znaleziony.");
    }
}
