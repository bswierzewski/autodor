using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Extensions;
using ErrorOr;
using InFaktClient = Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses.Client;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;

public class InFaktInvoiceService(InFaktHttpClient httpClient) : IInvoiceService
{
    public async Task<ErrorOr<Success>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        // Upsert client (ensure it exists in InFakt system and is up to date)
        var upsertResult = await UpsertClientAsync(invoice.Contractor, cancellationToken);
        if (upsertResult.IsError)
            return upsertResult.Errors;

        // Create invoice (InFakt will automatically link to client by NIP)
        return await CreateInvoiceInternalAsync(invoice, cancellationToken);
    }

    private async Task<ErrorOr<Success>> UpsertClientAsync(Contractor contractor, CancellationToken cancellationToken)
    {
        var searchQuery = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter { NipEq = contractor.NIP }
        };

        var clientsResult = await httpClient.GetClientsAsync(searchQuery, cancellationToken);
        if (clientsResult.IsError)
            return clientsResult.Errors;

        var existingClient = clientsResult.Value.Entities.FirstOrDefault(c => c.Id.HasValue);

        if (existingClient is not null)
        {
            // Client exists - update if needed
            if (RequiresUpdate(existingClient, contractor))
            {
                var updatedClient = contractor.ToInFaktClient();
                var updateResult = await httpClient.UpdateClientAsync(existingClient.Id!.Value, updatedClient, cancellationToken);
                return updateResult.IsError ? updateResult.Errors : Result.Success;
            }

            return Result.Success;
        }

        // Client doesn't exist - create new one
        var newClient = contractor.ToInFaktClient();
        var createResult = await httpClient.CreateClientAsync(newClient, cancellationToken);
        return createResult.IsError ? createResult.Errors : Result.Success;
    }

    private static bool RequiresUpdate(
        InFaktClient existingClient,
        Contractor contractor)
    {
        return !string.Equals(existingClient.CompanyName, contractor.Name, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(existingClient.Street, contractor.Street, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(existingClient.City, contractor.City, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(existingClient.PostalCode, contractor.ZipCode, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(existingClient.Nip, contractor.NIP, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(existingClient.Email, contractor.Email, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<ErrorOr<Success>> CreateInvoiceInternalAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var inFaktInvoice = invoice.ToInFaktInvoice();
        var result = await httpClient.CreateInvoiceAsync(inFaktInvoice, cancellationToken);
        return result.IsError ? result.Errors : Result.Success;
    }
}
