using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Features.GetContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Contractors.GetContractor;

[Collection(SharedCollection.Name)]
public class GetContractorTests(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{

    [Fact(Skip = "Disabled by default")]
    public async Task Should_Get_Contractor_By_Id()
    {
        // Arrange
        var contractorId = new ContractorId(Guid.NewGuid());

        await using (var arrangeScope = Host.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

            var contractor = new Contractor(
                contractorId,
                new TaxId("1234567890"),
                "Test Company",
                new Address("Test Street 1", "Warsaw", "00-001"),
                new Email("test@company.com")
            );

            db.Contractors.Add(contractor);
            await db.SaveChangesAsync();
        }

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url($"/contractors/{contractorId.Value}");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var response = await result.ReadAsJsonAsync<GetContractorResponse>();
        response.Should().NotBeNull();
        response.Id.Should().Be(contractorId.Value);
        response.Name.Should().Be("Test Company");
        response.NIP.Should().Be("1234567890");
        response.Email.Should().Be("test@company.com");
    }

    [Fact(Skip = "Disabled by default")]
    public async Task Should_Return_NotFound_When_Contractor_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
