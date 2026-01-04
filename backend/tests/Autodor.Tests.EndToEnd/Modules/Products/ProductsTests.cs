using Autodor.Shared.Contracts.Products;
using BuildingBlocks.Tests.Core;

namespace Autodor.Tests.EndToEnd.Modules.Products;

[Collection("Autodor")]
public class ProductsTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private TestContext _context = null!;

    public async Task InitializeAsync()
    {
        _context = await TestContext.CreateBuilder<Program>()
            .WithContainer(shared.Container)
            .BuildAsync();

        await _context.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }

    [Fact]
    public async Task GetProductByNumber_InDevelopmentEnvironment_ShouldReturnNullWithoutError()
    {
        // Arrange
        var productsApi = _context.GetRequiredService<IProductsAPI>();

        // Act
        var result = await productsApi.GetProductByNumberAsync("PROD-001");

        // Assert
        result.Should().BeNull("products synchronization is disabled in Development environment");
    }

    [Fact]
    public async Task GetProductsByNumbers_InDevelopmentEnvironment_ShouldReturnEmptyListWithoutError()
    {
        // Arrange
        var productsApi = _context.GetRequiredService<IProductsAPI>();
        var productNumbers = new[] { "PROD-001", "PROD-002", "PROD-003" };

        // Act
        var result = await productsApi.GetProductsByNumbersAsync(productNumbers);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty("products synchronization is disabled in Development environment");
    }

    [Fact]
    public async Task GetProductByNumber_WithNullOrEmptyNumber_ShouldReturnNull()
    {
        // Arrange
        var productsApi = _context.GetRequiredService<IProductsAPI>();

        // Act
        var resultEmpty = await productsApi.GetProductByNumberAsync(string.Empty);
        var resultWhitespace = await productsApi.GetProductByNumberAsync("   ");

        // Assert
        resultEmpty.Should().BeNull();
        resultWhitespace.Should().BeNull();
    }

    [Fact]
    public async Task GetProductsByNumbers_WithEmptyCollection_ShouldReturnEmptyList()
    {
        // Arrange
        var productsApi = _context.GetRequiredService<IProductsAPI>();
        var emptyNumbers = Array.Empty<string>();

        // Act
        var result = await productsApi.GetProductsByNumbersAsync(emptyNumbers);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ProductsAPI_ShouldBeRegisteredInDI()
    {
        // Arrange & Act
        var productsApi = _context.GetRequiredService<IProductsAPI>();

        // Assert
        productsApi.Should().NotBeNull("IProductsAPI should be registered in DI container");
    }
}
