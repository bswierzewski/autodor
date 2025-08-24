using Autodor.Shared.Contracts.Orders.Commands;
using Autodor.Shared.Contracts.Orders.Queries;
using MediatR;

namespace Autodor.Web.Endpoints;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (DateTime dateFrom, DateTime dateTo, Guid contractorId, IMediator mediator) =>
        {
            var orders = await mediator.Send(new GetOrdersQuery(dateFrom, dateTo, contractorId));
            return Results.Ok(orders);
        })
        .WithName("GetOrders")
        .WithSummary("Get orders for contractor in date range");

        group.MapGet("/by-ids", async (string[] orderNumbers, IMediator mediator) =>
        {
            var orders = await mediator.Send(new GetOrdersByIdsQuery(orderNumbers));
            return Results.Ok(orders);
        })
        .WithName("GetOrdersByIds")
        .WithSummary("Get orders by order numbers");

        group.MapGet("/by-contractor-and-period", async (Guid contractorId, DateTime dateFrom, DateTime dateTo, IMediator mediator) =>
        {
            var orders = await mediator.Send(new GetOrdersByContractorAndPeriodQuery(contractorId, dateFrom, dateTo));
            return Results.Ok(orders);
        })
        .WithName("GetOrdersByContractorAndPeriod")
        .WithSummary("Get orders for contractor in specified period");

        group.MapPost("/exclude", async (ExcludeOrderCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(new { Success = result });
        })
        .WithName("ExcludeOrder")
        .WithSummary("Exclude order from processing");

        return app;
    }
}