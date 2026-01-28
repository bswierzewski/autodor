using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Features.GetContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Contractors.GetContractor;

[Collection(SharedCollection.Name)]
public class GetContractorTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    [Fact]
    public async Task Should_Get_Contractor_By_Id()
    {
        // Arrange
        var db = GetRequiredService<ContractorsDbContext>();
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId("1234567890"),
            "Test Company",
            new Address("Test Street 1", "Warsaw", "00-001"),
            new Email("test@company.com")
        );

        db.Contractors.Add(contractor);
        await db.SaveChangesAsync();

        // Act
        var result = await AlbaHost.Scenario(s =>
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

    [Fact]
    public async Task Should_Return_NotFound_When_Contractor_Does_Not_Exist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await AlbaHost.Scenario(s =>
        {
            s.Get.Url($"/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
