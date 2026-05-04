using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Features.GetContractors;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Modules.Contractors.GetContractors;

[Collection(SharedCollection.Name)]
public class GetContractorsTests(SharedEnvironment Environment) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
        await SeedDataAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task SeedDataAsync()
    {
        await using var scope = Environment.Host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

        var contractor1 = new Contractor(
            new ContractorId(Guid.NewGuid()),
            new TaxId("1111111111"),
            "Company A",
            new Address("Street 1", "Warsaw", "00-001"),
            new Email("companyA@test.com")
        );

        var contractor2 = new Contractor(
            new ContractorId(Guid.NewGuid()),
            new TaxId("2222222222"),
            "Company B",
            new Address("Street 2", "Krakow", "30-001"),
            new Email("companyB@test.com")
        );

        db.Contractors.AddRange(contractor1, contractor2);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Should_Get_All_Contractors()
    {
        // Act
        var result = await Environment.Host.Scenario(s =>
        {
            s.Get.Url("/contractors");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var contractors = await result.ReadAsJsonAsync<List<GetContractorsResponse>>();
        contractors.Should().NotBeNull();
        contractors.Should().HaveCount(2);
        contractors.Should().Contain(c => c.Name == "Company A");
        contractors.Should().Contain(c => c.Name == "Company B");
    }

    [Fact]
    public async Task Should_Filter_Contractors_By_NIP()
    {
        // Act
        var result = await Environment.Host.Scenario(s =>
        {
            s.Get.Url("/contractors?NIPs=1111111111");
            s.StatusCodeShouldBe(200);
        });

        // Assert
        var contractors = await result.ReadAsJsonAsync<List<GetContractorsResponse>>();
        contractors.Should().NotBeNull();
        contractors.Should().HaveCount(1);
        contractors.First().Name.Should().Be("Company A");
        contractors.First().NIP.Should().Be("1111111111");
    }
}
