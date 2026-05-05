using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Modules.Orders.DistributorsSales;

[Collection(SharedCollection.Name)]
public class DistributorsSalesServiceTests(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{

    /// <summary>
    /// Manual test to verify real API integration.
    /// To run: Remove the Skip parameter and execute this test manually.
    /// </summary>
    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task GetOrdersAsync_ShouldReturnOrdersFromRealApi()
    {
        // Arrange
        await using var scope = Host.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IDistributorsSalesClient>();

        // Act
        var result = await service.GetOrdersAsync(new DateTime(2026, 02, 04));

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}
