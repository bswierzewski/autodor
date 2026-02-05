using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.PrintOrders;

public static class PrintOrders
{
    /// <summary>
    /// Command
    /// </summary>
    public record Command(
        Guid[] OrderIds
    );

    /// <summary>
    /// Handler
    /// </summary>
    public static class Handler
    {
        [WolverinePost("/orders/print")]
        [Tags("Orders")]
        public static async Task<IResult> Handle(
            Command command,
            OrdersDbContext dbContext,
            CancellationToken ct)
        {
            // TODO: Implement print logic
            // This might generate a PDF or prepare data for printing

            await Task.CompletedTask;
            return Results.Ok();
        }
    }

}

