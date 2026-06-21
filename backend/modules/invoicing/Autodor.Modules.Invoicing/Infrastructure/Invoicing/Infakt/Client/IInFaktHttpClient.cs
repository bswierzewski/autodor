using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Requests;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;
using Refit;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;

/// <summary>
/// Defines the InFakt HTTP API endpoints used by the invoicing module.
/// </summary>
public interface IInFaktHttpClient
{
    /// <summary>
    /// Creates an invoice in InFakt.
    /// </summary>
    [Post("/invoices.json")]
    Task<Models.Responses.Invoice> CreateInvoiceAsync(
        [Body] InvoiceRoot invoice,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a client in InFakt.
    /// </summary>
    [Post("/clients.json")]
    Task<Models.Responses.Client> CreateClientAsync(
        [Body] ClientRoot client,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an InFakt client by identifier.
    /// </summary>
    [Get("/clients/{clientId}.json")]
    Task<Models.Responses.Client> GetClientAsync(
        int clientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing InFakt client.
    /// </summary>
    [Put("/clients/{clientId}.json")]
    Task<Models.Responses.Client> UpdateClientAsync(
        int clientId,
        [Body] ClientRoot client,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for InFakt clients using the supplied filter.
    /// </summary>
    [Get("/clients.json")]
    Task<Clients> GetClientsAsync(
        [Body] ClientSearchQuery searchQuery,
        CancellationToken cancellationToken = default);
}
