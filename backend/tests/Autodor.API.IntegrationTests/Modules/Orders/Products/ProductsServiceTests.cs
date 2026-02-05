using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Orders.Products;

[Collection(SharedCollection.Name)]
public class ProductsServiceTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    /// <summary>
    /// Manual test to verify real API integration and caching.
    /// To run: Remove the Skip parameter and execute this test manually.
    /// </summary>
    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task RefreshAsync_ShouldLoadProductsIntoCache()
    {
        // Arrange
        var service = GetRequiredService<IProductsService>();

        // Act - Refresh cache from SOAP API
        var products = await service.GetProductsAsync();

        // Assert - Try to get some products from cache
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
    }
}
