using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Invoicing.Features.CreateInvoice;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Invoicing.CreateInvoice;

[Collection(SharedCollection.Name)]
public class CreateInvoiceTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    protected override async Task SeedDataAsync()
    {
        // Seed test contractor
        var db = GetRequiredService<ContractorsDbContext>();
        var contractor = new Contractor(
            id: new ContractorId(Guid.NewGuid()),
            name: "Test Company Ltd",
            nip: new TaxId("1234567890"),
            address: new Address("Test Street 1", "Warsaw", "00-001"),
            email: new Email("test@company.com")
        );
        db.Contractors.Add(contractor);
        await db.SaveChangesAsync();
    }

    [Fact(Skip = "Requires Orders module test data and InvoiceService configuration")]
    public async Task CreateInvoice_WithValidData_ShouldCreateInvoice()
    {
        // Arrange
        var command = new CreateInvoiceCommand(
            InvoiceNumber: null,
            SaleDate: DateTime.Today,
            IssueDate: DateTime.Today,
            Dates: [DateTime.Today],
            OrderIds: ["ORDER-001"],
            ContractorNip: "1234567890"
        );

        // Act
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices");
            s.StatusCodeShouldBe(200);
        });

        // Assert - verify invoice was created via external API
        // TODO: Check actual invoice creation response
    }

    [Fact(Skip = "Requires test data setup")]
    public async Task CreateInvoice_WithInvalidContractorNip_ShouldReturnNotFound()
    {
        // Arrange
        var command = new CreateInvoiceCommand(
            InvoiceNumber: null,
            SaleDate: DateTime.Today,
            IssueDate: DateTime.Today,
            Dates: [DateTime.Today],
            OrderIds: ["ORDER-001"],
            ContractorNip: "INVALID-NIP"
        );

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices");
            s.StatusCodeShouldBe(404); // Contractor not found
        });
    }

    [Fact(Skip = "Requires Orders module test data setup")]
    public async Task CreateInvoice_WithEmptyOrderIds_ShouldReturnNotFound()
    {
        // Arrange
        var command = new CreateInvoiceCommand(
            InvoiceNumber: null,
            SaleDate: DateTime.Today,
            IssueDate: DateTime.Today,
            Dates: [DateTime.Today],
            OrderIds: [], // Empty order IDs
            ContractorNip: "1234567890"
        );

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices");
            s.StatusCodeShouldBe(404); // No orders found
        });
    }
}
