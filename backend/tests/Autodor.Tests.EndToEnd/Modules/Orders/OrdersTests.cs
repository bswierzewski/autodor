using Autodor.Modules.Orders.Domain.Entities;
using BuildingBlocks.Tests.Extensions.Http;

namespace Autodor.Tests.EndToEnd.Modules.Orders;

[Collection("Autodor")]
public class OrdersTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
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

    [Fact]
    public async Task GetOrderById_WithValidOrderId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = "8b44ae93-23ea-f011-95f5-00155d0b7aef";

        // Act
        var response = await Context.Client.GetAsync($"/api/orders/{orderId}");
        var order = await response.ReadAsJsonAsync<Order>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        order.Should().NotBeNull();
        order.Id.Should().Be("8b44ae93-23ea-f011-95f5-00155d0b7aef");
    }
}
