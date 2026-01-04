using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

public class IFirmaHttpClient(HttpClient httpClient)
{
    public async Task<ResponseRoot> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        using var request = new HttpRequestMessage(HttpMethod.Post, "fakturakraj.json");

        request.SetApiKey(IFirmaKeyType.Invoice);
        
        request.Content = JsonContent.Create(invoice);

        
        using var response = await httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        
        var invoiceResponse = await response.Content.ReadFromJsonAsync<ResponseRoot>(cancellationToken: cancellationToken);

        return invoiceResponse ?? throw new InvalidOperationException(
            "Empty response from iFirma API when creating invoice.");
    }
}
