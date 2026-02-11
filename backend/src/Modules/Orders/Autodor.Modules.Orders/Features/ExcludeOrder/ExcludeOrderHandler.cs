using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public static class ExcludeOrderHandler
{
    [WolverinePatch("/orders/{id}")]
    [Tags("Orders")]
    [EndpointName("UpdateOrderExclusion")]
    [EndpointSummary("Include or exclude order from invoicing")]
    public static async Task<IResult> Handle(
        string id,
        ExcludeOrderCommand command,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        var excludedOrder = await dbContext.ExcludedOrders
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (command.Excluded)
        {
            // Exclude order - add to excluded list if not already there
            if (excludedOrder is null)
            {
                excludedOrder = new ExcludedOrder(id);
                await dbContext.ExcludedOrders.AddAsync(excludedOrder, ct);
            }
        }
        else
        {
            // Include order - remove from excluded list if present
            if (excludedOrder is not null)
            {
                dbContext.ExcludedOrders.Remove(excludedOrder);
            }
        }

        await dbContext.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}
