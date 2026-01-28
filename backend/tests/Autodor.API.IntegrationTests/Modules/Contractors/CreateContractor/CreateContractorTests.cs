using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Features.CreateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Autodor.API.IntegrationTests.Modules.Contractors.CreateContractor;

[Collection(SharedCollection.Name)]
public class CreateContractorTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    [Fact]
    public async Task Should_Create_Contractor()
    {
        // Arrange
        var command = new CreateContractorCommand(
            Name: "Test Company",
            NIP: "1234567890",
            Street: "Test Street 1",
            City: "Warsaw",
            ZipCode: "00-001",
            Email: "test@company.com"
        );

        // Act
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/contractors");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var response = await result.ReadAsJsonAsync<CreateContractorResponse>();
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        var db = GetRequiredService<ContractorsDbContext>();
        var contractorId = new Autodor.Modules.Contractors.Domain.ValueObjects.ContractorId(response.Id);
        var contractor = await db.Contractors.FirstOrDefaultAsync(c => c.Id == contractorId);
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Test Company");
        contractor.NIP.Value.Should().Be("1234567890");
    }
}
