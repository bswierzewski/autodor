using Autodor.Modules.Contractors.Features.CreateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Modules.Contractors.CreateContractor;

[Collection(SharedCollection.Name)]
public class CreateContractorTests(SharedEnvironment Environment) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

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
        var result = await Environment.Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/contractors");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var response = await result.ReadAsJsonAsync<CreateContractorResponse>();
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        await using var scope = Environment.Host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var contractorId = new Autodor.Modules.Contractors.Domain.ValueObjects.ContractorId(response.Id);
        var contractor = await db.Contractors.FirstOrDefaultAsync(c => c.Id == contractorId);
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Test Company");
        contractor.NIP.Value.Should().Be("1234567890");
    }
}
