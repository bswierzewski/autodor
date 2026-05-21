using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public static class ExcludeOrderHandler
{
    public static async Task<IResult> Handle(
        ExcludeOrderCommand command,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        var excludedOrder = await dbContext.ExcludedOrders
            .FirstOrDefaultAsync(o => o.Id == command.Id, ct);

        if (command.Excluded)
        {
            // Exclude order - add to excluded list if not already there
            if (excludedOrder is null)
            {
                excludedOrder = new ExcludedOrder(command.Id);
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
