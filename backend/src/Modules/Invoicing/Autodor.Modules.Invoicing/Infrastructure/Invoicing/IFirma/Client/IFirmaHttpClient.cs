using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Responses;
using BuildingBlocks.Infrastructure.Http;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

public class IFirmaHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    public async Task<ErrorOr<ResponseRoot>> CreateInvoiceAsync(Models.Requests.Invoice invoice, CancellationToken ct = default)
    {
        if (invoice is null)
            return Error.Validation(nameof(invoice), "Invoice cannot be null.");

        var result = await PostAsync<Models.Requests.Invoice, ResponseRoot>("fakturakraj.json", invoice, ct);

        if (result.IsError)
            return result.Errors;

        if (!result.Value.Response.IsSuccess)
            return Error.Failure("IFirma.ApiError", result.Value.Response.Message ?? "Unknown error from iFirma API.");

        return result;
    }

    protected override async Task<List<Error>> ParseErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        return [Error.Failure("IFirma.HttpError", $"IFirma API error: {content}")];
    }
}
