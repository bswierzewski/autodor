using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace Autodor.Tests.Integration.Contractors.GetContractorsByNIPs;

public class GetContractorsByNIPsTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Manual test - skipped by default")]
    public async Task Should_Filter_Contractors_By_Nips()
    {
        // Arrange
        await using (var arrangeScope = Host.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

            db.Contractors.AddRange(
                new Contractor(
                    new ContractorId(Guid.NewGuid()),
                    new TaxId("1111111111"),
                    "Company A",
                    new Address("Street 1", "Warsaw", "00-001"),
                    new Email("companyA@test.com")),
                new Contractor(
                    new ContractorId(Guid.NewGuid()),
                    new TaxId("2222222222"),
                    "Company B",
                    new Address("Street 2", "Krakow", "30-001"),
                    new Email("companyB@test.com"))
            );

            await db.SaveChangesAsync();
        }

        var bus = Host.Services.GetRequiredService<IMessageBus>();

        // Act
        var response = (await bus.InvokeAsync<IEnumerable<ContractorDto>>(
            new GetContractorsByNIPsQuery(["1111111111"]),
            CancellationToken.None)).ToList();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCount(1);
        response[0].Name.Should().Be("Company A");
        response[0].NIP.Should().Be("1111111111");
    }
}
