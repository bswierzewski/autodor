using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Features.UpdateContractor;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Modules.Contractors.UpdateContractor;

[Collection(SharedCollection.Name)]
public class UpdateContractorTests(SharedEnvironment Environment) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Should_Update_Contractor()
    {
        // Arrange
        var contractorId = new ContractorId(Guid.NewGuid());

        await using (var arrangeScope = Environment.Host.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

            var contractor = new Contractor(
                contractorId,
                new TaxId("1234567890"),
                "Old Company Name",
                new Address("Old Street", "Old City", "00-000"),
                new Email("old@company.com")
            );

            db.Contractors.Add(contractor);
            await db.SaveChangesAsync();
        }

        var request = new UpdateContractorCommand(
            NIP: "9876543210",
            Name: "New Company Name",
            Street: "New Street 123",
            City: "New City",
            ZipCode: "11-111",
            Email: "new@company.com"
        );

        // Act
        await Environment.Host.Scenario(s =>
        {
            s.Put.Json(request).ToUrl($"/contractors/{contractorId.Value}");
            s.StatusCodeShouldBe(204);
        });

        // Assert
        await using var assertScope = Environment.Host.Services.CreateAsyncScope();
        var assertDb = assertScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();
        var updatedContractor = await assertDb.Contractors
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
        await Environment.Host.Scenario(s =>
        {
            s.Put.Json(request).ToUrl($"/contractors/{nonExistentId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
