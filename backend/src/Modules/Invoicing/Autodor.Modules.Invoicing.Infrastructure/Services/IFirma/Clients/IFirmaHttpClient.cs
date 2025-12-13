using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Responses;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

/// <summary>
/// HTTP client for the iFirma API.
/// Authentication is handled automatically by the IFirmaAuthenticationHandler,
/// which looks for the API key type set on the request via the SetApiKey extension method.
/// </summary>
public class IFirmaHttpClient(HttpClient httpClient)
{
    /// <summary>
    /// Creates a domestic invoice in iFirma.
    /// </summary>
    /// <param name="invoice">The invoice data to be created.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>An Envelope containing the invoice response from iFirma if successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when invoice is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the iFirma API returns an empty response.</exception>
    public async Task<Envelope> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        // Create the request with the appropriate HTTP method and endpoint
        using var request = new HttpRequestMessage(HttpMethod.Post, "fakturakraj.json");
        // Set the API key type for this request
        request.SetApiKey(IFirmaKeyType.Invoice);
        // Set the request content (JSON serialized invoice)
        request.Content = JsonContent.Create(invoice);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);
        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from iFirma
        var invoiceResponse = await response.Content.ReadFromJsonAsync<Envelope>(cancellationToken: cancellationToken);

        return invoiceResponse ?? throw new InvalidOperationException(
            "Empty response from iFirma API when creating invoice.");
    }
}
