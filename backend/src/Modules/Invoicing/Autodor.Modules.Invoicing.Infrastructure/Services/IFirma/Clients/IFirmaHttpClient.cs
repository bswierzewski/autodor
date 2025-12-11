using System.Net.Http.Json;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

/// <summary>
/// HTTP client for iFirma API.
/// Authentication is handled automatically based on URL route mapping.
/// </summary>
public class IFirmaHttpClient(HttpClient httpClient)
{
    private const string CreateInvoiceEndpoint = "/iapi/fakturakraj.json";

    /// <summary>
    /// Creates a domestic invoice in iFirma.
    /// </summary>
    /// <param name="invoice">Invoice data to be created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from iFirma API containing invoice details if successful.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<Envelope> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        using var content = JsonContent.Create(invoice);
        using var response = await httpClient.PostAsync(CreateInvoiceEndpoint, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var invoiceResponse = await response.Content.ReadFromJsonAsync<Envelope>(cancellationToken: cancellationToken);
        return invoiceResponse ?? throw new InvalidOperationException("Empty response from iFirma API when creating invoice.");
    }
}
