using Microsoft.AspNetCore.Http;
using Wolverine.Http;

namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public class CreateInvoicesHandler
{
    [WolverinePost("/invoices")]
    [Tags("Invoicing")]
    public static async Task<IResult> Handle(
        CreateInvoicesCommand command,
        CancellationToken ct)
    {
        // TODO: Implement invoice creation logic

        await Task.CompletedTask;
        return Results.Ok();
    }
}
