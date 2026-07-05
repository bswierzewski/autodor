using System.Net;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.Integration.Orders.ExcludeOrderItem;

public class ExcludeOrderItemTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrderItem_WithValidIds_ShouldExcludeOrderItem()
    {
        // Arrange
        var orderId = "TEST-ORDER-456";
        var itemNumber = "ITEM-789";

        // Act
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{itemNumber}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert - Verify order item was excluded in database
        await using var scope = Host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var excludedOrderItem = await dbContext.ExcludedOrderItems
            .FirstOrDefaultAsync(i => i.OrderId == orderId && i.ItemNumber == itemNumber);

        Assert.NotNull(excludedOrderItem);
        Assert.Equal(orderId, excludedOrderItem.OrderId);
        Assert.Equal(itemNumber, excludedOrderItem.ItemNumber);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrderItem_WhenCalledTwice_ShouldToggleExclusion()
    {
        // Arrange
        var orderId = "TOGGLE-ORDER-ITEM";
        var itemNumber = "TOGGLE-ITEM";

        // Act 1 - First call should exclude the order item
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{itemNumber}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify order item is excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == itemNumber);
            Assert.True(excluded);
        }

        // Act 2 - Second call should restore the order item (remove from excluded list)
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{itemNumber}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify order item is no longer excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == itemNumber);
            Assert.False(excluded);
        }

        // Act 3 - Third call should exclude the order item again
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{itemNumber}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify order item is excluded again
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            var excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == itemNumber);
            Assert.True(excluded);
        }
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrderItem_WhenOrderIdIsEmpty_ShouldReturnValidationError()
    {
        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Post.Url("/api/orders/ /items/ITEM-123/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrderItem_WhenItemNumberIsEmpty_ShouldReturnValidationError()
    {
        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Post.Url("/api/orders/ORDER-123/items/ /exclude");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact(Skip = "Disabled by default")]
    public async Task ExcludeOrderItem_WithMultipleItems_ShouldExcludeOnlySpecificItem()
    {
        // Arrange - Order with two items
        var orderId = "MULTI-ITEM-ORDER";
        var item1Number = "ITEM-001";
        var item2Number = "ITEM-002";

        // Act 1 - Exclude only the first item
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{item1Number}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 1 - Verify only item1 is excluded, item2 is not
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var item1Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item1Number);
            var item2Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item2Number);

            Assert.True(item1Excluded, "Item 1 should be excluded");
            Assert.False(item2Excluded, "Item 2 should NOT be excluded");
        }

        // Act 2 - Restore the first item (toggle off)
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{item1Number}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 2 - Verify both items are now not excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var item1Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item1Number);
            var item2Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item2Number);

            Assert.False(item1Excluded, "Item 1 should be restored (not excluded)");
            Assert.False(item2Excluded, "Item 2 should still NOT be excluded");
        }

        // Act 3 - Exclude the second item this time
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{item2Number}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 3 - Verify only item2 is excluded now
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var item1Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item1Number);
            var item2Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item2Number);

            Assert.False(item1Excluded, "Item 1 should still be restored (not excluded)");
            Assert.True(item2Excluded, "Item 2 should now be excluded");
        }

        // Act 4 - Exclude both items
        await Host.Scenario(s =>
        {
            s.Post.Url($"/api/orders/{orderId}/items/{item1Number}/exclude");
            s.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });

        // Assert 4 - Verify both items are excluded
        await using (var scope = Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var item1Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item1Number);
            var item2Excluded = await dbContext.ExcludedOrderItems
                .AnyAsync(i => i.OrderId == orderId && i.ItemNumber == item2Number);

            Assert.True(item1Excluded, "Item 1 should be excluded again");
            Assert.True(item2Excluded, "Item 2 should still be excluded");
        }
    }
}
