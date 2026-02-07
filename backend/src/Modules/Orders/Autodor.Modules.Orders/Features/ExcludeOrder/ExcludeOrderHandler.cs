using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public static class ExcludeOrderHandler
{
    [WolverinePost("/orders/{orderId}/exclude")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        [AsParameters] ExcludeOrderCommand command,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // Toggle exclusion - if exists, remove it (include), otherwise add it (exclude)
        var excludedOrder = await dbContext.ExcludedOrders
            .FirstOrDefaultAsync(o => o.Id == command.OrderId, ct);

        if (excludedOrder is not null)
        {
            // Order is excluded - restore it (remove from excluded list)
            dbContext.ExcludedOrders.Remove(excludedOrder);
        }
        else
        {
            // Order is not excluded - exclude it (add to excluded list)
            excludedOrder = new ExcludedOrder(command.OrderId);
            await dbContext.ExcludedOrders.AddAsync(excludedOrder, ct);
        }

        await dbContext.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
