using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

public static class ExcludeOrderItem
{
    /// <summary>
    /// Handler
    /// </summary>
    public static class Handler
    {
        [WolverinePost("/orders/{orderId}/items/{itemId}/exclude")]
        [Tags("Orders")]
        public static async Task<IResult> Handle(
            Guid orderId,
            Guid itemId,
            OrdersDbContext dbContext,
            CancellationToken ct)
        {
            // TODO: Implement when Order entity is created
            // var order = await dbContext.Orders.FindAsync([orderId], ct);
            // if (order == null)
            //     return Results.NotFound();

            // order.ExcludeItem(itemId);

            await Task.CompletedTask;
            return Results.Ok();
        }
    }
}
