using Autodor.Modules.Orders.Domain.Entities;
using BuildingBlocks.Tests.Extensions.Http;

namespace Autodor.Tests.EndToEnd.Modules.Orders;

[Collection("Autodor")]
public class GetOrdersTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    [Fact]
    public async Task GetOrders_WithFromDateOnly_ShouldReturnOrderCollection()
    {
        // Arrange
        var from = new DateTime(2026, 1, 5);

        // Act
        var response = await Context.Client.GetAsync($"/api/orders?from={from:yyyy-MM-dd}");
        var orders = await response.ReadAsJsonAsync<List<Order>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        orders.Should().NotBeNull();

    }

    [Fact]
    public async Task GetOrders_WithDateRange_ShouldReturnOrderCollection()
    {
        // Arrange
        var from = new DateTime(2026, 1, 1);
        var to = new DateTime(2026, 1, 5);

        // Act
        var response = await Context.Client.GetAsync($"/api/orders?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        var orders = await response.ReadAsJsonAsync<List<Order>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        orders.Should().NotBeNull();
    }
}
