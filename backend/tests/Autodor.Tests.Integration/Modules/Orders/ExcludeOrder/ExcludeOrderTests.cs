using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Autodor.Tests.Integration.Modules.Orders.ExcludeOrder;

[Collection(SharedCollection.Name)]
public class ExcludeOrderTests(SharedEnvironment Environment) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task ExcludeOrder_WithValidOrderId_ShouldExcludeOrder()
    {
        // Arrange
        var orderId = "TEST-ORDER-123";

        // Act
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert - Verify order was excluded in database
        await using var scope = Environment.Host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var excludedOrder = await dbContext.ExcludedOrders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        Assert.NotNull(excludedOrder);
        Assert.Equal(orderId, excludedOrder.Id);
    }

    [Fact]
    public async Task ExcludeOrder_WhenCalledTwice_ShouldToggleExclusion()
    {
        // Arrange
        var orderId = "TOGGLE-ORDER";

        // Act 1 - First call should exclude the order
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify order is excluded
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.True(excluded);
        }

        // Act 2 - Second call should restore the order (remove from excluded list)
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify order is no longer excluded
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.False(excluded);
        }

        // Act 3 - Third call should exclude the order again
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{orderId}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify order is excluded again
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrders
                .AnyAsync(o => o.Id == orderId);
            Assert.True(excluded);
        }
    }

    [Fact]
    public async Task ExcludeOrder_WhenOrderIdIsEmpty_ShouldReturnValidationError()
    {
        // Act && Assert
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url("/orders/ /exclude");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact]
    public async Task ExcludeOrder_WithMultipleOrders_ShouldExcludeOnlySpecificOrder()
    {
        // Arrange - Multiple orders
        var order1Id = "ORDER-001";
        var order2Id = "ORDER-002";

        // Act 1 - Exclude only the first order
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify only order1 is excluded, order2 is not
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
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
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify both orders are now not excluded
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
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
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{order2Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify only order2 is excluded now
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
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
        await Environment.Host.Scenario(s =>
        {
            s.Post.Url($"/orders/{order1Id}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 4 - Verify both orders are excluded
        await using (var scope = Environment.Host.Services.CreateAsyncScope())
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
