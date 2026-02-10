using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Responses;
using BuildingBlocks.Infrastructure.Http;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

public class IFirmaHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    public async Task<ResponseRoot> CreateInvoiceAsync(Models.Requests.Invoice invoice, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var result = await PostAsync<Models.Requests.Invoice, ResponseRoot>("fakturakraj.json", invoice, ct);

        ValidateResponse(result);

        return result;
    }

    protected override async Task ParseErrorAndThrowAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        throw new InvalidOperationException($"IFirma API error: {content}");
    }

    private static void ValidateResponse(ResponseRoot response)
    {
        if (!response.Response.IsSuccess)
            throw new InvalidOperationException(response.Response.Message ?? "Unknown error from iFirma API.");
    }
}
