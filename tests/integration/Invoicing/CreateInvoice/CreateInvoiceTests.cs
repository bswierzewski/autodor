using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Invoicing.Features.CreateInvoice;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Invoicing.CreateInvoice;

[Collection(SharedCollection.Name)]
public class CreateInvoiceTests(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{
    protected override async Task OnInitializeAsync(IServiceProvider services)
    {
        await SeedDataAsync();
    }

    private async Task SeedDataAsync()
    {
        // Seed test contractor
        await using var scope = Host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var contractor = new Contractor(
            id: new ContractorId(Guid.NewGuid()),
            name: "Test Company Ltd",
            nip: new TaxId("1190712364"),
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
            Dates: [new DateTime(2026, 2, 5)],
            OrderIds: ["3ff0615c-b902-f111-95f5-00155d0b7aef"],
            ContractorNIP: "1190712364"
        );

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoices");
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
            ContractorNIP: "INVALID-NIP"
        );

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoices");
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
            OrderIds: [],
            ContractorNIP: "1234567890"
        );

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoices");
            s.StatusCodeShouldBe(404); // No orders found
        });
    }
}
