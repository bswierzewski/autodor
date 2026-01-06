using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Requests;
using ErrorOr;
using System.Net.Http.Json;
using Responses = Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Responses;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;

public class InFaktHttpClient(HttpClient httpClient)
{
    private static class ErrorCodes
    {
        public const string CreateInvoiceFailed = "InFakt.CreateInvoiceFailed";
        public const string CreateClientFailed = "InFakt.CreateClientFailed";
        public const string GetClientFailed = "InFakt.GetClientFailed";
        public const string GetClientsFailed = "InFakt.GetClientsFailed";
        public const string UpdateClientFailed = "InFakt.UpdateClientFailed";
        public const string EmptyResponse = "InFakt.EmptyResponse";
    }

    public async Task<ErrorOr<Responses.Invoice>> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var request = new HttpRequestMessage(HttpMethod.Post, "invoices.json")
        {
            Content = JsonContent.Create(new InvoiceRoot(invoice))
        };

        return await SendRequestAsync<Responses.Invoice>(
            request,
            ErrorCodes.CreateInvoiceFailed,
            cancellationToken);
    }

    public async Task<ErrorOr<Responses.Client>> CreateClientAsync(
        Client client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        using var request = new HttpRequestMessage(HttpMethod.Post, "clients.json")
        {
            Content = JsonContent.Create(new ClientRoot(client))
        };

        return await SendRequestAsync<Responses.Client>(
            request,
            ErrorCodes.CreateClientFailed,
            cancellationToken);
    }

    public async Task<ErrorOr<Responses.Client>> GetClientAsync(
        int clientId,
        CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
            return Error.Failure(ErrorCodes.GetClientFailed, "Client ID must be greater than 0.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"clients/{clientId}.json");

        return await SendRequestAsync<Responses.Client>(
            request,
            ErrorCodes.GetClientFailed,
            cancellationToken);
    }

    public async Task<ErrorOr<Responses.ClientList>> GetClientsAsync(
        ClientSearchQuery searchQuery,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "clients.json")
        {
            Content = JsonContent.Create(searchQuery)
        };

        return await SendRequestAsync<Responses.ClientList>(
            request,
            ErrorCodes.GetClientsFailed,
            cancellationToken);
    }

    public async Task<ErrorOr<Responses.Client>> UpdateClientAsync(
        int clientId,
        Client client,
        CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
            return Error.Failure(ErrorCodes.UpdateClientFailed, "Client ID must be greater than 0.");

        ArgumentNullException.ThrowIfNull(client);

        using var request = new HttpRequestMessage(HttpMethod.Put, $"clients/{clientId}.json")
        {
            Content = JsonContent.Create(new ClientRoot(client))
        };

        return await SendRequestAsync<Responses.Client>(
            request,
            ErrorCodes.UpdateClientFailed,
            cancellationToken);
    }

    private async Task<ErrorOr<TResponse>> SendRequestAsync<TResponse>(
        HttpRequestMessage request,
        string errorCode,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return await ParseErrorResponseAsync(response, errorCode, cancellationToken);

        var responseData = await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken);

        return responseData is null
            ? Error.Failure(ErrorCodes.EmptyResponse, $"Empty response from InFakt API.")
            : responseData;
    }

    private async Task<List<Error>> ParseErrorResponseAsync(
        HttpResponseMessage response,
        string errorCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<Responses.ErrorResponse>(
                cancellationToken: cancellationToken);

            if (errorResponse?.Errors is { Count: > 0 })
                return CreateValidationErrors(errorResponse.Errors, errorCode);

            if (!string.IsNullOrEmpty(errorResponse?.Error))
                return [Error.Failure(errorCode, errorResponse.Error)];
        }
        catch
        {
            // If we can't parse as ErrorResponse, fall back to raw content
        }

        var rawError = await response.Content.ReadAsStringAsync(cancellationToken);
        return [Error.Failure(errorCode, $"Status: {response.StatusCode}, Error: {rawError}")];
    }

    private static List<Error> CreateValidationErrors(
        Dictionary<string, List<string>> errors,
        string baseErrorCode)
    {
        return errors
            .SelectMany(kvp => kvp.Value.Select(message =>
                Error.Validation($"{baseErrorCode}.{kvp.Key}", message)))
            .ToList();
    }
}
