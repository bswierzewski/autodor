using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Invoicing.Features.CreateInvoice;

public class CreateInvoiceHandler
{
    [WolverinePost("/invoicing/invoices")]
    [Tags("Invoicing")]
    public static async Task<IResult> Handle(
        CreateInvoiceCommand command,
        CancellationToken ct)
    {
        // TODO: Implement invoice creation logic

        await Task.CompletedTask;
        return Results.Ok();
    }
}
