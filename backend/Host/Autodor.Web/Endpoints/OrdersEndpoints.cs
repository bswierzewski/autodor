using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autodor.Web.Endpoints;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost("/exclude", ExcludeOrder)
            .WithName("ExcludeOrder")
            .WithOpenApi();

        group.MapGet("/by-date", GetOrdersByDate)
            .WithName("GetOrdersByDate")
            .WithOpenApi();

        group.MapGet("/by-date-range", GetOrdersByDateRange)
            .WithName("GetOrdersByDateRange")
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> ExcludeOrder(
        [FromBody] ExcludeOrderCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.Ok(new { Success = result });
    }

    private static async Task<IResult> GetOrdersByDate(
        DateTime date,
        IMediator mediator)
    {
        var query = new GetOrdersByDateQuery(date);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrdersByDateRange(
        DateTime dateFrom,
        DateTime dateTo,
        IMediator mediator)
    {
        var query = new GetOrdersByDateRangeQuery(dateFrom, dateTo);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }
}