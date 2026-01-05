using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Autodor.Tests.EndToEnd.Modules.Orders;

[Collection("Autodor")]
public class ExcludeOrderTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    [Fact]
    public async Task ExcludeOrder_WithValidOrderId_ShouldReturnNoContent()
    {
        // Arrange
        var orderId = "8514de01-42ea-f011-95f5-00155d0b7aef";
        var command = new ExcludeOrderCommand(orderId);

        // Act
        var response = await Context.Client.PostAsJsonAsync($"/api/orders/{orderId}/exclude", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify in database
        var readContext = Context.GetRequiredService<IOrdersDbContext>();
        var excludedOrdersCount = await readContext.ExcludedOrders.CountAsync();
        excludedOrdersCount.Should().Be(1);

        var excludedOrder = await readContext.ExcludedOrders.FirstAsync();
        excludedOrder.OrderId.Should().Be(orderId);
        excludedOrder.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
