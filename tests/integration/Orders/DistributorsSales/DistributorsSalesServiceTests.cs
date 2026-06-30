using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Orders.DistributorsSales;

public class DistributorsSalesServiceTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
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
