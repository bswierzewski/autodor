using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateInvoice;

public static class CreateInvoiceEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/invoices", Handle)
            .RequireAuthorization()
            .WithTags("Invoicing")
            .WithName("CreateInvoice")
            .WithSummary("Create a single invoice for selected orders");
    }

    private static Task<IResult> Handle(
        CreateInvoiceCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<IResult>(command, ct);
    }
}
