using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;
using BuildingBlocks.Infrastructure.Http;
using ErrorOr;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;

public class InFaktHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    public async Task<ErrorOr<Models.Responses.Invoice>> CreateInvoiceAsync(Models.Requests.Invoice invoice, CancellationToken ct = default)
    {
        if (invoice is null)
            return Error.Validation(nameof(invoice), "Invoice cannot be null.");

        return await PostAsync<InvoiceRoot, Models.Responses.Invoice>("invoices.json", new InvoiceRoot(invoice), ct);
    }

    public async Task<ErrorOr<Models.Responses.Client>> CreateClientAsync(Models.Requests.Client client, CancellationToken ct = default)
    {
        if (client is null)
            return Error.Validation(nameof(client), "Client cannot be null.");

        return await PostAsync<ClientRoot, Models.Responses.Client>("clients.json", new ClientRoot(client), ct);
    }

    public async Task<ErrorOr<Models.Responses.Client>> GetClientAsync(int clientId, CancellationToken ct = default)
    {
        if (clientId <= 0)
            return Error.Validation(nameof(clientId), "Client ID must be greater than zero.");

        return await GetAsync<Models.Responses.Client>($"clients/{clientId}.json", ct);
    }

    public async Task<ErrorOr<Models.Responses.Client>> UpdateClientAsync(int clientId, Models.Requests.Client client, CancellationToken ct = default)
    {
        if (clientId <= 0)
            return Error.Validation(nameof(clientId), "Client ID must be greater than zero.");

        if (client is null)
            return Error.Validation(nameof(client), "Client cannot be null.");

        return await PutAsync<ClientRoot, Models.Responses.Client>($"clients/{clientId}.json", new ClientRoot(client), ct);
    }

    public async Task<ErrorOr<Clients>> GetClientsAsync(ClientSearchQuery searchQuery, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "clients.json")
        {
            Content = JsonContent.Create(searchQuery)
        };

        return await SendRequestAsync<Clients>(request, ct);
    }

    protected override async Task<List<Error>> ParseErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(ct);

        if (errorResponse?.Errors is null || errorResponse.Errors.Count == 0)
            return [Error.Failure("InFakt.ApiError", errorResponse?.Error ?? "Unknown error from InFakt API")];

        var errors = errorResponse.Errors
            .SelectMany(field => field.Value.Select(msg => Error.Validation(field.Key, msg)))
            .ToList();

        return errors;
    }
}