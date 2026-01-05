using Autodor.Modules.Orders.Domain.Entities;
using BuildingBlocks.Tests.Extensions.Http;

namespace Autodor.Tests.EndToEnd.Modules.Orders;

[Collection("Autodor")]
public class GetOrderByIdTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    [Fact]
    public async Task GetOrderById_WithValidOrderId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = "8514de01-42ea-f011-95f5-00155d0b7aef";

        // Act
        var response = await Context.Client.GetAsync($"/api/orders/{orderId}");
        var order = await response.ReadAsJsonAsync<Order>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        order.Should().NotBeNull();
    }
}
