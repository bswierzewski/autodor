using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;
using BuildingBlocks.Core.Exceptions;
using System.Net.Http.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Handlers;

/// <summary>
/// Converts unsuccessful InFakt responses into application exceptions.
/// </summary>
public class InFaktErrorHandler : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
            return response;

        using (response)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);

            if (errorResponse?.Errors is null || errorResponse.Errors.Count == 0)
                throw new Exception(errorResponse?.Error ?? "Nieznany błąd z API InFakt.");

            var errors = errorResponse.Errors.ToDictionary(
                field => field.Key,
                field => field.Value.ToArray());

            throw new ValidationException(errors);
        }
    }
}
