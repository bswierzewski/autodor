using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

public class ExcludeOrderHandler
{
    [WolverinePost("/orders/{orderId}/exclude")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        Guid orderId,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // TODO: Implement when Order entity is created
        // var order = await dbContext.Orders.FindAsync([orderId], ct);
        // if (order == null)
        //     return Results.NotFound();

        // order.Exclude();

        await Task.CompletedTask;
        return Results.Ok();
    }
}
