using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace Autodor.Tests.Integration.Contractors.GetContractorByNip;

public class GetContractorByNipTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Manual test - skipped by default")]
    public async Task Should_Get_Contractor_By_Nip()
    {
        // Arrange
        await using (var arrangeScope = Host.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

            var contractor = new Contractor(
                new ContractorId(Guid.NewGuid()),
                new TaxId("1234567890"),
                "Test Company",
                new Address("Test Street 1", "Warsaw", "00-001"),
                new Email("test@company.com")
            );

            db.Contractors.Add(contractor);
            await db.SaveChangesAsync();
        }

        var bus = Host.Services.GetRequiredService<IMessageBus>();

        // Act
        var response = await bus.InvokeAsync<ContractorDto?>(
            new GetContractorByNIPQuery("1234567890"),
            CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be("Test Company");
        response.NIP.Should().Be("1234567890");
        response.Email.Should().Be("test@company.com");
    }
}
