using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Requests;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using FluentValidation;

namespace Autodor.API.IntegrationTests.Modules.Invoicing.Clients;

[Collection(SharedCollection.Name)]
public class InFaktHttpClientTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    private InFaktHttpClient Client => GetRequiredService<InFaktHttpClient>();

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
    public async Task GetClientsAsync_WithEmptyFilter_ShouldReturnClientList()
    {
        // Arrange
        var query = new ClientSearchQuery();

        // Act
        var result = await Client.GetClientsAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Entities.Should().NotBeNull();
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
    public async Task GetClientsAsync_WithNipFilter_ShouldReturnMatchingClients()
    {
        // Arrange - Using a valid test NIP
        var query = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter { NipEq = "1176224556" }
        };

        // Act
        var result = await Client.GetClientsAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Entities.Should().NotBeNull();
        // Filter may return 0 results if client doesn't exist in sandbox
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
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

        var createdClient = await Client.CreateClientAsync(newClient);
        createdClient.Id.Should().NotBeNull();

        // Act
        var result = await Client.GetClientAsync(createdClient.Id!.Value);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("GetTest");
        result.Id.Should().Be(createdClient.Id);
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
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
        var result = await Client.CreateClientAsync(client);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("User");
        result.Id.Should().NotBeNull();
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
    public async Task UpdateClientAsync_WithValidClient_ShouldUpdateAndReturnClient()
    {
        // Arrange - First create a test client
        var newClient = new Client
        {
            Country = "PL",
            FirstName = "UpdateTest",
            LastName = "User",
            BusinessActivityKind = "private_person",
            Email = $"update-test-{Guid.NewGuid()}@example.com"
        };

        var createdClient = await Client.CreateClientAsync(newClient);
        createdClient.Id.Should().NotBeNull();

        // Update the client data
        var updatedClient = new Client
        {
            Country = "PL",
            FirstName = "UpdatedTest",
            LastName = "UpdatedUser",
            BusinessActivityKind = "private_person",
            Email = createdClient.Email
        };

        // Act
        var result = await Client.UpdateClientAsync(createdClient.Id!.Value, updatedClient);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("UpdatedTest");
        result.LastName.Should().Be("UpdatedUser");
        result.Id.Should().Be(createdClient.Id);
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithValidInvoice_ShouldCreateInvoice()
    {
        // Arrange
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
        var result = await Client.CreateInvoiceAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNull();
        result.Status.Should().BeOneOf("draft", "sent", "printed", "paid");
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
    public async Task CreateInvoiceAsync_WithInvalidInvoice_ShouldReturnError()
    {
        // Arrange
        var invoice = new Invoice
        {
            Currency = "PLN",
            Notes = "Test invoice from API",
            Kind = "vats", // Invalid value
            PaymentMethod = "transfera", // Invalid value
            ClientTaxCode = "117622455622", // Invalid NIP
            InvoiceDate = DateTime.Now.ToString("yyyy-dd-MM"), // Invalid format
            SaleDate = DateTime.Now.ToString("yyyy-MM-dd"),
            Services = new List<InvoiceItem>
            {
                new()
                {
                    Name = "Test Service",
                    TaxSymbol = "231", // Invalid value
                    Unit = "szta", // Invalid value
                    Quantity = -1, // Invalid value
                    UnitNetPrice = 10000
                }
            }
        };

        // Act
        var act = () => Client.CreateInvoiceAsync(invoice);

        // Assert
        await act.Should().ThrowAsync<ValidationException>("API should return error for invalid data");
    }

    [Fact(Skip = "Manual test - requires real InFakt API connection and valid credentials")]
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
        var act = () => Client.CreateInvoiceAsync(invoice);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>("API should return error for missing required field");
        exception.Which.Errors.Should().NotBeEmpty();
        exception.Which.Errors.Should().Contain(e => e.PropertyName == "services", "Error should include the services field");
        exception.Which.Errors.First(e => e.PropertyName == "services").ErrorMessage.Should().Contain("Proszę dodać pozycję na fakturze.");
    }
}
