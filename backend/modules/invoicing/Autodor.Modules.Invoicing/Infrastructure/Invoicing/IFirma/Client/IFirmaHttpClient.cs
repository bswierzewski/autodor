using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Responses;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Http;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

public class IFirmaHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    public async Task<ResponseRoot> CreateInvoiceAsync(Models.Requests.Invoice invoice, CancellationToken ct = default)
    {
        if (invoice is null)
            throw CreateValidationException(nameof(invoice), "Invoice cannot be null.");

        var result = await PostAsync<Models.Requests.Invoice, ResponseRoot>("fakturakraj.json", invoice, ct);

        if (!result.Response.IsSuccess)
            throw new Exception(result.Response.Message ?? "Unknown error from iFirma API.");

        return result;
    }

    protected override async Task<Exception> ParseExceptionAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct);
        return new Exception($"IFirma API error: {content}");
    }

    private static ValidationException CreateValidationException(string key, string message)
    {
        return new ValidationException(new Dictionary<string, string[]>
        {
            [key] = [message]
        });
    }
}
