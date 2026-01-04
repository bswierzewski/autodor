using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using System.Net.Http.Json;
using Requests = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using Responses = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Responses;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;

public class InFaktHttpClient(HttpClient httpClient)
{
    public async Task<Responses.Invoice> CreateInvoiceAsync(
        Requests.Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

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

    public async Task<Responses.Client> CreateClientAsync(
        Requests.Client client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

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

    public async Task<Responses.Client> UpdateClientAsync(
        int clientId,
        Requests.Client client,
        CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client ID must be greater than 0.", nameof(clientId));

        ArgumentNullException.ThrowIfNull(client);

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
