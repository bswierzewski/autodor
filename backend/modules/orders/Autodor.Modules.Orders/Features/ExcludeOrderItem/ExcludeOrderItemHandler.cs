using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Domain.ValueObjects;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

public static class ExcludeOrderItemHandler
{
    public static async Task<IResult> Handle(
        ExcludeOrderItemCommand command,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        var excludedOrderItem = await dbContext.ExcludedOrderItems
            .FirstOrDefaultAsync(i => i.OrderId == command.Id && i.ItemNumber == command.ItemNumber, ct);

        if (command.Excluded)
        {
            // Exclude item - add to excluded list if not already there
            if (excludedOrderItem is null)
            {
                var itemId = new OrderItemId(command.Id, command.ItemNumber);
                excludedOrderItem = new ExcludedOrderItem(itemId);
                await dbContext.ExcludedOrderItems.AddAsync(excludedOrderItem, ct);
            }
        }
        else
        {
            // Include item - remove from excluded list if present
            if (excludedOrderItem is not null)
            {
                dbContext.ExcludedOrderItems.Remove(excludedOrderItem);
            }
        }

        await dbContext.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
