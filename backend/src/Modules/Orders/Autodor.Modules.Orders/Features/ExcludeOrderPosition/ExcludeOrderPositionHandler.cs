using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrderPosition;

public class ExcludeOrderPositionHandler
{
    [WolverinePost("/orders/{orderId}/{positionId}/exclude")]
    [Tags("Orders")]
    public static async Task<IResult> Handle(
        Guid orderId,
        Guid positionId,
        OrdersDbContext dbContext,
        CancellationToken ct)
    {
        // TODO: Implement when Order entity is created
        // var order = await dbContext.Orders.FindAsync([orderId], ct);
        // if (order == null)
        //     return Results.NotFound();

        // order.ExcludePosition(positionId);

        await Task.CompletedTask;
        return Results.Ok();
    }
}
