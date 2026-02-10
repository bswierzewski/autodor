using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Invoicing.Features.CreateInvoices;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Invoicing.CreateInvoices;

[Collection(SharedCollection.Name)]
public class CreateInvoicesTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    protected override async Task SeedDataAsync()
    {
        // Seed test contractors
        var db = GetRequiredService<ContractorsDbContext>();
        var contractor1 = new Contractor(
            id: new ContractorId(Guid.NewGuid()),
            name: "Test Company A",
            nip: new TaxId("1234567890"),
            address: new Address("Street A", "Warsaw", "00-001"),
            email: new Email("companyA@test.com")
        );
        var contractor2 = new Contractor(
            id: new ContractorId(Guid.NewGuid()),
            name: "Test Company B",
            nip: new TaxId("9876543210"),
            address: new Address("Street B", "Krakow", "30-001"),
            email: new Email("companyB@test.com")
        );
        db.Contractors.AddRange(contractor1, contractor2);
        await db.SaveChangesAsync();
    }

    [Fact(Skip = "Requires Orders module test data and InvoiceService configuration")]
    public async Task CreateInvoices_WithValidDateRange_ShouldCreateMultipleInvoices()
    {
        // Arrange
        var command = new CreateInvoicesCommand(
            DateFrom: DateTime.Today.AddDays(-7),
            DateTo: DateTime.Today
        );

        // Act
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices/bulk");
            s.StatusCodeShouldBe(200);
        });

        // Assert - verify actual API response
        var response = await result.ReadAsJsonAsync<CreateInvoicesResponse>();
        response.Should().NotBeNull();
        response.InvoicesCreated.Should().BeGreaterThan(0);
    }

    [Fact(Skip = "Requires test data setup")]
    public async Task CreateInvoices_WithNoOrdersInDateRange_ShouldReturnNotFound()
    {
        // Arrange
        var command = new CreateInvoicesCommand(
            DateFrom: DateTime.Today.AddYears(-10),
            DateTo: DateTime.Today.AddYears(-9) // Very old date range with no orders
        );

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices/bulk");
            s.StatusCodeShouldBe(404); // No orders found
        });
    }

    [Fact(Skip = "Requires Orders module test data and InvoiceService configuration")]
    public async Task CreateInvoices_WithSingleDay_ShouldCreateInvoicesForThatDay()
    {
        // Arrange
        var command = new CreateInvoicesCommand(
            DateFrom: DateTime.Today,
            DateTo: DateTime.Today // Same day
        );

        // Act
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/invoicing/invoices/bulk");
            s.StatusCodeShouldBe(200);
        });

        // Assert - verify actual API response
        var response = await result.ReadAsJsonAsync<CreateInvoicesResponse>();
        response.Should().NotBeNull();
    }
}

public record CreateInvoicesResponse(int InvoicesCreated);
