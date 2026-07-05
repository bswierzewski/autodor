using System.Net;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Orders.ExcludeOrder;

public class ExcludeOrderTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrder_WithValidOrderId_ShouldExcludeOrder()
    {
        // Arrange
        var orderId = "TEST-ORDER-123";

        // Act
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert - Verify order was excluded in database
        await using var scope = Host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var excludedOrder = await dbContext.ExcludedOrders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        Assert.NotNull(excludedOrder);
        Assert.Equal(orderId, excludedOrder.Id);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrder_WhenCalledTwice_ShouldToggleExclusion()
    {
        // Arrange
        var orderId = "TOGGLE-ORDER";

        // Act 1 - First call should exclude the order
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify order is excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.True(excluded);
        }

        // Act 2 - Second call should restore the order (remove from excluded list)
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify order is no longer excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.False(excluded);
        }

        // Act 3 - Third call should exclude the order again
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify order is excluded again
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.True(excluded);
        }
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrder_WhenOrderIdIsEmpty_ShouldReturnValidationError()
    {
        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Post.Url("/api/orders/ /exclude");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrder_WithMultipleOrders_ShouldExcludeOnlySpecificOrder()
    {
        // Arrange - Multiple orders
        var order1Id = "ORDER-001";
        var order2Id = "ORDER-002";

        // Act 1 - Exclude only the first order
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify only order1 is excluded, order2 is not
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order1Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order1Id);
            var order2Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order2Id);

            Assert.True(order1Excluded, "Order 1 should be excluded");
            Assert.False(order2Excluded, "Order 2 should NOT be excluded");
        }

        // Act 2 - Restore the first order (toggle off)
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify both orders are now not excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order1Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order1Id);
            var order2Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order2Id);

            Assert.False(order1Excluded, "Order 1 should be restored (not excluded)");
            Assert.False(order2Excluded, "Order 2 should still NOT be excluded");
        }

        // Act 3 - Exclude the second order this time
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{order2Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify only order2 is excluded now
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order1Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order1Id);
            var order2Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order2Id);

            Assert.False(order1Excluded, "Order 1 should still be restored (not excluded)");
            Assert.True(order2Excluded, "Order 2 should now be excluded");
        }

        // Act 4 - Exclude both orders
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 4 - Verify both orders are excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order1Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order1Id);
            var order2Excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == order2Id);

            Assert.True(order1Excluded, "Order 1 should be excluded again");
            Assert.True(order2Excluded, "Order 2 should still be excluded");
        }
    }
}
