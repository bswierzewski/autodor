using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Autodor.Modules.Orders.Application.Queries.GetOrderById;
using Autodor.Modules.Orders.Application.Queries.GetOrders;
using BuildingBlocks.Infrastructure.Extensions;
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
        var result = await mediator.Send(command);

        return result.ToNoContentResult();
    }

    private static async Task<IResult> GetOrders(
        [FromQuery] DateTime from,
        [FromQuery] DateTime? to,
        IMediator mediator)
    {
        var query = new GetOrdersQuery(from, to);
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetOrderById(
        string orderId,
        IMediator mediator)
    {
        var query = new GetOrderByIdQuery(orderId);
        var result = await mediator.Send(query);

        return result.ToHttpResult();
    }
}
