using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrders;

public static class GetOrders
{
    /// <summary>
    /// Query
    /// </summary>
    public record Query(
        DateOnly? From,
        DateOnly? To
    );

    /// <summary>
    /// Handler
    /// </summary>
    public static class Handler
    {
        [WolverineGet("/orders")]
        [Tags("Orders")]
        public static async Task<IResult> Handle(
            [AsParameters] Query query,
            OrdersDbContext dbContext,
            CancellationToken ct)
        {
            // TODO: Implement when Order entity is created
            // var queryable = dbContext.Orders.AsNoTracking();
            // if (query.From.HasValue)
            //     queryable = queryable.Where(o => o.Date >= query.From.Value);
            // if (query.To.HasValue)
            //     queryable = queryable.Where(o => o.Date <= query.To.Value);

            await Task.CompletedTask;
            return Results.Ok();
        }
    }
}
