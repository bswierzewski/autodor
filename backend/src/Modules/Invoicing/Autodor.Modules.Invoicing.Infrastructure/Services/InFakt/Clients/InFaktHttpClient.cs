using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using System.Net.Http.Json;
using Requests = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using Responses = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Responses;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;

/// <summary>
/// HTTP client for InFakt API.
/// </summary>
public class InFaktHttpClient(HttpClient httpClient)
{
    /// <summary>
    /// Creates an invoice in InFakt.
    /// </summary>
    /// <param name="invoice">Invoice data to be created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from InFakt API containing invoice details if successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when invoice is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<Responses.Invoice> CreateInvoiceAsync(
        Requests.Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        // Create the request with the appropriate HTTP method and endpoint
        using var request = new HttpRequestMessage(HttpMethod.Post, "invoices.json");

        // Set the request content (JSON serialized invoice request)
        var invoiceRequest = new InvoiceRoot(invoice);
        request.Content = JsonContent.Create(invoiceRequest);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);

        // {"error":"Wadliwy zasób. Popraw błędy i spróbuj ponownie.","errors":{"number":["Masz już wystawione faktury dla późniejszych dat i wystawienie faktury z wybraną datą spowoduje niespójność w numeracji."]}}
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from InFakt
        var invoiceResponse = await response.Content.ReadFromJsonAsync<Responses.Invoice>(cancellationToken: cancellationToken);
        return invoiceResponse ?? throw new InvalidOperationException("Empty response from InFakt API when creating invoice.");
    }

    /// <summary>
    /// Creates a client in InFakt.
    /// </summary>
    /// <param name="client">Client data to be created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from InFakt API containing client details if successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<Responses.Client> CreateClientAsync(
        Requests.Client client,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        // Create the request with the appropriate HTTP method and endpoint
        using var request = new HttpRequestMessage(HttpMethod.Post, "clients.json");

        // Set the request content (JSON serialized client request)
        var clientRequest = new ClientRoot(client);
        request.Content = JsonContent.Create(clientRequest);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);

        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from InFakt
        var clientResponse = await response.Content.ReadFromJsonAsync<Responses.Client>(cancellationToken: cancellationToken);
        return clientResponse ?? throw new InvalidOperationException("Empty response from InFakt API when creating client.");
    }

    /// <summary>
    /// Retrieves a client from InFakt by ID.
    /// </summary>
    /// <param name="clientId">The ID of the client to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from InFakt API containing client details if found.</returns>
    /// <exception cref="ArgumentException">Thrown when clientId is invalid.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or client is not found (404).</exception>
    public async Task<Responses.Client> GetClientAsync(
        int clientId,
        CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client ID must be greater than 0.", nameof(clientId));

        // Create the request with the appropriate HTTP method and endpoint
        var endpoint = $"clients/{clientId}.json";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);

        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from InFakt
        var clientResponse = await response.Content.ReadFromJsonAsync<Responses.Client>(cancellationToken: cancellationToken);
        return clientResponse ?? throw new InvalidOperationException("Empty response from InFakt API when retrieving client.");
    }

    /// <summary>
    /// Retrieves a list of clients from InFakt with optional filtering.
    /// </summary>
    /// <param name="searchQuery">Search query with filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from InFakt API containing list of clients matching the filter.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<Responses.ClientList> GetClientsAsync(
        ClientSearchQuery searchQuery,
        CancellationToken cancellationToken = default)
    {
        // Create the request with the appropriate HTTP method and endpoint
        using var request = new HttpRequestMessage(HttpMethod.Get, "clients.json");

        // Set the request content (JSON serialized search query)
        request.Content = JsonContent.Create(searchQuery);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);

        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from InFakt
        var clientListResponse = await response.Content.ReadFromJsonAsync<Responses.ClientList>(cancellationToken: cancellationToken);
        return clientListResponse ?? throw new InvalidOperationException("Empty response from InFakt API when listing clients.");
    }

    /// <summary>
    /// Updates an existing client in InFakt.
    /// </summary>
    /// <param name="clientId">The ID of the client to update.</param>
    /// <param name="client">Updated client data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response from InFakt API containing updated client details if successful.</returns>
    /// <exception cref="ArgumentException">Thrown when clientId is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<Responses.Client> UpdateClientAsync(
        int clientId,
        Requests.Client client,
        CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client ID must be greater than 0.", nameof(clientId));

        if (client == null)
            throw new ArgumentNullException(nameof(client));

        // Create the request with the appropriate HTTP method and endpoint
        var endpoint = $"clients/{clientId}.json";
        using var request = new HttpRequestMessage(HttpMethod.Put, endpoint);

        // Set the request content (JSON serialized client request)
        var clientRequest = new ClientRoot(client);
        request.Content = JsonContent.Create(clientRequest);

        // Send the request
        using var response = await httpClient.SendAsync(request, cancellationToken);

        // Ensure the response indicates success (2xx status code)
        response.EnsureSuccessStatusCode();

        // Deserialize the response from InFakt
        var clientResponse = await response.Content.ReadFromJsonAsync<Responses.Client>(cancellationToken: cancellationToken);
        return clientResponse ?? throw new InvalidOperationException("Empty response from InFakt API when updating client.");
    }
}
