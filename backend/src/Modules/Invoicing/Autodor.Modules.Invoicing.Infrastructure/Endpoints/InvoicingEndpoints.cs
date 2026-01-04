using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
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

        group.MapPost("/create", CreateInvoice)
            .WithName("CreateInvoice");

        group.MapPost("/create-bulk", CreateBulkInvoices)
            .WithName("CreateBulkInvoices");

        return endpoints;
    }

    private static async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand command,
        IMediator mediator)
    {
        try
        {
            await mediator.Send(command);

            return Results.Created("/api/invoicing", new { Success = true });
        }
        catch (Exception)
        {
            return Results.BadRequest();
        }
    }

    private static async Task<IResult> CreateBulkInvoices(
        [FromBody] CreateBulkInvoicesCommand command,
        IMediator mediator)
    {
        var invoiceStatuses = await mediator.Send(command);

        return Results.Ok(new { InvoiceStatuses = invoiceStatuses, Count = invoiceStatuses?.Count() ?? 0 });
    }
}
