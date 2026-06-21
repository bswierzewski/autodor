using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Contractors.DeleteContractor;

[Collection(SharedCollection.Name)]
public class DeleteContractorTests(AutodorDatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{

    [Fact(Skip = "Disabled by default")]
    public async Task Should_Delete_Contractor()
    {
        // Arrange
        var contractorId = new ContractorId(Guid.NewGuid());

        await using (var arrangeScope = Host.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

            var contractor = new Contractor(
                contractorId,
                new TaxId("1234567890"),
                "Company To Delete",
                new Address("Test Street 1", "Warsaw", "00-001"),
                new Email("delete@company.com")
            );

            db.Contractors.Add(contractor);
            await db.SaveChangesAsync();
        }

        // Act
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/api/contractors/{contractorId.Value}");
            s.StatusCodeShouldBe(204);
        });

        // Assert
        await using var assertScope = Host.Services.CreateAsyncScope();
        var assertDb = assertScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var deletedContractor = await assertDb.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contractorId);

        deletedContractor.Should().BeNull();
    }

    [Fact(Skip = "Disabled by default")]
    public async Task Should_Return_NotFound_When_Contractor_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/api/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
