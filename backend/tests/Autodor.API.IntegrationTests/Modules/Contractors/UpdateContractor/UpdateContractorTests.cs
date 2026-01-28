using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Features.UpdateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autodor.API.IntegrationTests.Modules.Contractors.UpdateContractor;

[Collection(SharedCollection.Name)]
public class UpdateContractorTests(DatabaseFixture databaseFixture) : BuildingBlocks.IntegrationTests.TestBase<Program>(databaseFixture)
{

    [Fact]
    public async Task Should_Update_Contractor()
    {
        // Arrange
        var db = GetRequiredService<ContractorsDbContext>();
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId("1234567890"),
            "Old Company Name",
            new Address("Old Street", "Old City", "00-000"),
            new Email("old@company.com")
        );

        db.Contractors.Add(contractor);
        await db.SaveChangesAsync();

        var request = new UpdateContractorCommand(
            NIP: "9876543210",
            Name: "New Company Name",
            Street: "New Street 123",
            City: "New City",
            ZipCode: "11-111",
            Email: "new@company.com"
        );

        // Act
        await AlbaHost.Scenario(s =>
        {
            s.Put.Json(request).ToUrl($"/contractors/{contractorId.Value}");
            s.StatusCodeShouldBe(204);
        });

        // Assert
        var updatedContractor = await db.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contractorId);

        updatedContractor.Should().NotBeNull();
        updatedContractor!.Name.Should().Be("New Company Name");
        updatedContractor.NIP.Value.Should().Be("9876543210");
        updatedContractor.Address.Street.Should().Be("New Street 123");
        updatedContractor.Address.City.Should().Be("New City");
        updatedContractor.Address.ZipCode.Should().Be("11-111");
        updatedContractor.Email.Value.Should().Be("new@company.com");
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Contractor_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        var request = new UpdateContractorCommand(
            NIP: "9876543210",
            Name: "New Company Name",
            Street: "New Street 123",
            City: "New City",
            ZipCode: "11-111",
            Email: "new@company.com"
        );

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Put.Json(request).ToUrl($"/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
