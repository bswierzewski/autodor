using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Modules.Orders.Products;

[Collection(SharedCollection.Name)]
public class ProductsServiceTests(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{

    /// <summary>
    /// Manual test to verify real API integration and caching.
    /// To run: Remove the Skip parameter and execute this test manually.
    /// </summary>
    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task RefreshAsync_ShouldLoadProductsIntoCache()
    {
        // Arrange
        await using var scope = Host.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IProductsClient>();

        // Act - Refresh cache from SOAP API
        var products = await service.GetProductsAsync();

        // Assert - Try to get some products from cache
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
    }
}
