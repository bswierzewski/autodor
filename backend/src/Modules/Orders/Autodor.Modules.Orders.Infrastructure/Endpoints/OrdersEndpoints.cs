using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Autodor.Modules.Orders.Application.Queries.GetOrderById;
using Autodor.Modules.Orders.Application.Queries.GetOrders;
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

        group.MapGet("/", GetOrders)
            .WithName("GetOrders");

        group.MapGet("/{orderId}", GetOrderById)
            .WithName("GetOrderById");

        group.MapPost("/{orderId}/exclude", ExcludeOrder)
            .WithName("ExcludeOrder");

        return endpoints;
    }

    private static async Task<IResult> ExcludeOrder(
        string orderId,
        [FromBody] ExcludeOrderCommand command,
        IMediator mediator)
    {
        var success = await mediator.Send(command);

        return Results.Ok(new { Success = success });
    }

    private static async Task<IResult> GetOrders(
        [FromQuery] DateTime from,
        [FromQuery] DateTime? to,
        IMediator mediator)
    {
        var query = new GetOrdersQuery(from, to);
        var orders = await mediator.Send(query);
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetOrderById(
        string orderId,
        IMediator mediator)
    {
        var query = new GetOrderByIdQuery(orderId);
        var order = await mediator.Send(query);

        if (order is null)
        {
            return Results.NotFound($"Order with ID '{orderId}' not found");
        }

        return Results.Ok(order);
    }
}
