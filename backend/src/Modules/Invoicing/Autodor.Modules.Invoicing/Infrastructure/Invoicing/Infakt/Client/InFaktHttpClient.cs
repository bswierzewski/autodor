using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;
using BuildingBlocks.Infrastructure.Http;
using FluentValidation;
using FluentValidation.Results;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;

public class InFaktHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    public async Task<Models.Responses.Invoice> CreateInvoiceAsync(Models.Requests.Invoice invoice, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        return await PostAsync<InvoiceRoot, Models.Responses.Invoice>("invoices.json", new InvoiceRoot(invoice), ct);
    }

    public async Task<Models.Responses.Client> CreateClientAsync(Models.Requests.Client client, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        return await PostAsync<ClientRoot, Models.Responses.Client>("clients.json", new ClientRoot(client), ct);
    }

    public async Task<Models.Responses.Client> GetClientAsync(int clientId, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientId);

        return await GetAsync<Models.Responses.Client>($"clients/{clientId}.json", ct);
    }

    public async Task<Models.Responses.Client> UpdateClientAsync(int clientId, Models.Requests.Client client, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientId);

        return await PutAsync<ClientRoot, Models.Responses.Client>($"clients/{clientId}.json", new ClientRoot(client), ct);
    }

    public async Task<Clients> GetClientsAsync(ClientSearchQuery searchQuery, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(searchQuery);

        using var request = new HttpRequestMessage(HttpMethod.Get, "clients.json")
        {
            Content = JsonContent.Create(searchQuery)
        };

        return await SendRequestAsync<Clients>(request, ct);
    }

    protected override async Task ParseErrorAndThrowAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(ct);

        if (errorResponse?.Errors is null || errorResponse.Errors.Count == 0)        
            throw new InvalidOperationException(errorResponse?.Error ?? "Unknown error from InFakt API");        

        var failures = errorResponse.Errors
            .SelectMany(field => field.Value.Select(msg => new ValidationFailure(field.Key, msg)))
            .ToList();

        throw new ValidationException(failures);
    }
}