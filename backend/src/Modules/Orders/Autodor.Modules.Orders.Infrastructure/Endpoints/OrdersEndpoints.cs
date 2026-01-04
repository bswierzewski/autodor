using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Autodor.Modules.Orders.Infrastructure.Endpoints;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/exclude", ExcludeOrder)
            .WithName("ExcludeOrder");

        group.MapGet("/by-date", GetOrdersByDate)
            .WithName("GetOrdersByDate");

        group.MapGet("/by-date-range", GetOrdersByDateRange)
            .WithName("GetOrdersByDateRange");

        return endpoints;
    }

    private static async Task<IResult> ExcludeOrder(
        [FromBody] ExcludeOrderCommand command,
        IMediator mediator)
    {
        var success = await mediator.Send(command);

        return Results.Ok(new { Success = success });
    }

    private static async Task<IResult> GetOrdersByDate(
        DateTime date,
        IMediator mediator)
    {
        var query = new GetOrdersByDateQuery(date);
        var orders = await mediator.Send(query);

        return Results.Ok(orders);
    }

    private static async Task<IResult> GetOrdersByDateRange(
        DateTime dateFrom,
        DateTime dateTo,
        IMediator mediator)
    {
        var query = new GetOrdersByDateRangeQuery(dateFrom, dateTo);
        var orders = await mediator.Send(query);

        return Results.Ok(orders);
    }
}
