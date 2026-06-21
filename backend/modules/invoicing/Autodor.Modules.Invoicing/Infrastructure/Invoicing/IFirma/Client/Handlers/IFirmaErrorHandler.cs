using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Responses;
using System.Text;
using System.Text.Json;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Handlers;

/// <summary>
/// Converts unsuccessful HTTP responses and iFirma business errors into exceptions.
/// </summary>
public class IFirmaErrorHandler : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            using (response)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Błąd API iFirma: {errorContent}");
            }
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<ResponseRoot>(content);

        if (result is not null && !result.Response.IsSuccess)
        {
            response.Dispose();
            throw new Exception(result.Response.Message ?? "Nieznany błąd z API iFirma.");
        }

        response.Content = new StringContent(content, Encoding.UTF8, "application/json");
        return response;
    }
}
