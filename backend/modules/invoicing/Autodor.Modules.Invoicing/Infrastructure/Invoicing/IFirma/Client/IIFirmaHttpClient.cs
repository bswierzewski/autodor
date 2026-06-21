using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Authentication;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client.Models.Responses;
using Refit;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

/// <summary>
/// Defines the iFirma HTTP API endpoints used by the invoicing module.
/// </summary>
public interface IIFirmaHttpClient
{
    /// <summary>
    /// Creates a domestic invoice in iFirma.
    /// </summary>
    [IFirmaKey(IFirmaKeyType.Invoice)]
    [Post("/fakturakraj.json")]
    Task<ResponseRoot> CreateInvoiceAsync(
        [Body] Models.Requests.Invoice invoice,
        CancellationToken cancellationToken = default);
}
