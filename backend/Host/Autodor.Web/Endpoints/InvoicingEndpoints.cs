using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using MediatR;

namespace Autodor.Web.Endpoints;

public static class InvoicingEndpoints
{
    public static IEndpointRouteBuilder MapInvoicingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoicing").WithTags("Invoicing");

        group.MapPost("/create", async (CreateInvoiceCommand command, IMediator mediator) =>
        {
            var invoiceId = await mediator.Send(command);
            return Results.Created($"/api/invoicing/{invoiceId}", new { InvoiceId = invoiceId });
        })
        .WithName("CreateInvoice")
        .WithSummary("Create invoice from orders");

        group.MapPost("/create-bulk", async (CreateBulkInvoicesCommand command, IMediator mediator) =>
        {
            var invoiceIds = await mediator.Send(command);
            return Results.Ok(new { InvoiceIds = invoiceIds, Count = invoiceIds.Count() });
        })
        .WithName("CreateBulkInvoices")
        .WithSummary("Create bulk invoices for contractor in date range");

        group.MapGet("/health", () => Results.Ok(new 
        { 
            Service = "Invoicing", 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow 
        }))
        .WithName("InvoicingHealth")
        .WithSummary("Health check for invoicing service");

        return app;
    }
}