using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateManualInvoice;

public static class CreateManualInvoiceEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/invoices/manual", Handle)
            .RequireAuthorization()
            .WithTags("Invoicing")
            .WithName("CreateManualInvoice")
            .WithSummary("Create an invoice from manually provided items");
    }

    private static Task<IResult> Handle(
        CreateManualInvoiceCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<IResult>(command, ct);
    }
}
