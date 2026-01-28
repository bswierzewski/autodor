using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autodor.API.IntegrationTests.Modules.Contractors.DeleteContractor;

[Collection(SharedCollection.Name)]
public class DeleteContractorTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    [Fact]
    public async Task Should_Delete_Contractor()
    {
        // Arrange
        var db = GetRequiredService<ContractorsDbContext>();
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId("1234567890"),
            "Company To Delete",
            new Address("Test Street 1", "Warsaw", "00-001"),
            new Email("delete@company.com")
        );

        db.Contractors.Add(contractor);
        await db.SaveChangesAsync();

        // Act
        await AlbaHost.Scenario(s =>
        {
            s.Delete.Url($"/contractors/{contractorId.Value}");
            s.StatusCodeShouldBe(204);
        });

        // Assert
        var deletedContractor = await db.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contractorId);

        deletedContractor.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Contractor_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Delete.Url($"/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
