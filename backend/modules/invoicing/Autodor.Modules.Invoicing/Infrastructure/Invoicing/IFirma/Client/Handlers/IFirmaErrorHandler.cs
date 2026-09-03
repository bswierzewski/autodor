namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Handlers;

/// <summary>
/// Converts unsuccessful HTTP responses into exceptions.
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

        return response;
    }
}
