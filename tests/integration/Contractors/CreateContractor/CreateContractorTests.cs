using Autodor.Modules.Contractors.Features.CreateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using BuildingBlocks.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Contractors.CreateContractor;

[Collection(SharedCollection.Name)]
public class CreateContractorTests(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{
    [Fact]
    public async Task Should_Create_Contractor_When_User_Is_Authenticated()
    {
        var command = new CreateContractorCommand(
            Name: "Authenticated Test Company",
            NIP: "1234567892",
            Street: "Test Street 1",
            City: "Warsaw",
            ZipCode: "00-001",
            Email: "authenticated-test@company.com"
        );

        var result = await Host.Scenario(s =>
        {
            s.As(new TestCurrentUser());
            s.Post.Json(command).ToUrl("/api/contractors");
            s.StatusCodeShouldBe(200);
        });

        var response = await result.ReadAsJsonAsync<CreateContractorResponse>();
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        await using var scope = Host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var contractorId = new Modules.Contractors.Domain.ValueObjects.ContractorId(response.Id);
        var contractor = await db.Contractors.FirstOrDefaultAsync(c => c.Id == contractorId);
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Authenticated Test Company");
        contractor.NIP.Value.Should().Be("1234567892");
    }

    [Fact(Skip = "Manual test - skipped by default")]
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
        var result = await Host.Scenario(s =>
        {
            s.As(new TestCurrentUser());
            s.Post.Json(command).ToUrl("/api/contractors");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var response = await result.ReadAsJsonAsync<CreateContractorResponse>();
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        await using var scope = Host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var contractorId = new Modules.Contractors.Domain.ValueObjects.ContractorId(response.Id);
        var contractor = await db.Contractors.FirstOrDefaultAsync(c => c.Id == contractorId);
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Test Company");
        contractor.NIP.Value.Should().Be("1234567890");
    }
}
