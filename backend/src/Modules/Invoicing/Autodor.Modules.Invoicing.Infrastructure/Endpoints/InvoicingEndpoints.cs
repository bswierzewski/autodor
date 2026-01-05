using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoices;
using BuildingBlocks.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Autodor.Modules.Invoicing.Infrastructure.Endpoints;

public static class InvoicingEndpoints
{
    public static IEndpointRouteBuilder MapInvoicingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/invoicing")
            .WithTags("Invoicing");

        group.MapPost("/", CreateInvoice)
            .WithName("CreateInvoice");

        group.MapPost("/bulk", CreateInvoices)
            .WithName("CreateInvoices");

        return endpoints;
    }

    private static async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);

        return result.ToCreatedResult("/api/invoicing");
    }

    private static async Task<IResult> CreateInvoices(
        [FromBody] CreateInvoicesCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);

        return result.ToHttpResult();
    }
}
