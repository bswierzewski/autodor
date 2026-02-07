using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Domain.ValueObjects;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

public static class ExcludeOrderItemHandler
{
    [WolverinePost("/orders/{orderId}/items/{itemNumber}/exclude")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        [AsParameters] ExcludeOrderItemCommand command,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // Toggle exclusion - if exists, remove it (include), otherwise add it (exclude)
        var excludedOrderItem = await dbContext.ExcludedOrderItems
            .FirstOrDefaultAsync(i => i.OrderId == command.OrderId && i.ItemNumber == command.ItemNumber, ct);

        if (excludedOrderItem is not null)
        {
            // Item is excluded - restore it (remove from excluded list)
            dbContext.ExcludedOrderItems.Remove(excludedOrderItem);
        }
        else
        {
            // Item is not excluded - exclude it (add to excluded list)
            var itemId = new OrderItemId(command.OrderId, command.ItemNumber);
            excludedOrderItem = new ExcludedOrderItem(itemId);
            await dbContext.ExcludedOrderItems.AddAsync(excludedOrderItem, ct);
        }

        await dbContext.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
