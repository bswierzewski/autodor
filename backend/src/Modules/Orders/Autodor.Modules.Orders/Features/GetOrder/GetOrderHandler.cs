using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GetOrder;

public class GetOrderHandler
{
    [WolverineGet("/orders/{orderId}")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        Guid orderId,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // TODO: Implement when Order entity is created
        // var order = await dbContext.Orders
        //     .AsNoTracking()
        //     .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        // if (order == null)
        //     return Results.NotFound();

        await Task.CompletedTask;
        return Results.Ok();
    }
}
