using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public static class CreateInvoicesEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/invoices/bulk", Handle)
            .WithTags("Invoicing")
            .WithName("CreateInvoicesBulk")
            .WithSummary("Create multiple invoices for date range");
    }

    private static Task<IResult> Handle(
        CreateInvoicesCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<IResult>(command, ct);
    }
}