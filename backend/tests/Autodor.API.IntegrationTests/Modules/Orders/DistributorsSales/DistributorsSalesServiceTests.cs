using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Orders.DistributorsSales;

[Collection(SharedCollection.Name)]
public class DistributorsSalesServiceTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    /// <summary>
    /// Manual test to verify real API integration.
    /// To run: Remove the Skip parameter and execute this test manually.
    /// </summary>
    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task GetOrdersAsync_ShouldReturnOrdersFromRealApi()
    {
        // Arrange
        var service = GetRequiredService<IDistributorsSalesService>();

        // Act
        var result = await service.GetOrdersAsync(new DateTime(2026, 02, 04));

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}
