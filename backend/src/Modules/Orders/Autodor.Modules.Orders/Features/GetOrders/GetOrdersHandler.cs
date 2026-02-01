using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrders;

public class GetOrdersHandler
{
    [WolverineGet("/orders")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        DateOnly? from,
        DateOnly? to,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // TODO: Implement when Order entity is created
        // var queryable = dbContext.Orders.AsNoTracking();
        // if (from.HasValue)
        //     queryable = queryable.Where(o => o.Date >= from.Value);
        // if (to.HasValue)
        //     queryable = queryable.Where(o => o.Date <= to.Value);

        await Task.CompletedTask;
        return Results.Ok();
    }
}
