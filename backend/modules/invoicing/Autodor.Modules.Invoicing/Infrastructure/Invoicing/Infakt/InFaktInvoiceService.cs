using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Extensions;
using InFaktClient = Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses.Client;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;

public class InFaktInvoiceService(IInFaktHttpClient httpClient) : IInvoiceService
{
    public async Task CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        // Upsert client (ensure it exists in InFakt system and is up to date)
        await UpsertClientAsync(invoice.Contractor, cancellationToken);

        // Create invoice (InFakt will automatically link to client by NIP)
        await CreateInvoiceInternalAsync(invoice, cancellationToken);
    }

    private async Task UpsertClientAsync(Contractor contractor, CancellationToken cancellationToken)
    {
        var searchQuery = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter { NipEq = contractor.NIP }
        };

        var clientsResult = await httpClient.GetClientsAsync(searchQuery, cancellationToken);

        var existingClient = clientsResult.Entities.FirstOrDefault(c => c.Id.HasValue);

        if (existingClient is not null)
        {
            // Client exists - update if needed
            if (RequiresUpdate(existingClient, contractor))
            {
                var updatedClient = contractor.ToInFaktClient();
                await httpClient.UpdateClientAsync(existingClient.Id!.Value, new(updatedClient), cancellationToken);
            }

            return;
        }

        // Client doesn't exist - create new one
        var newClient = contractor.ToInFaktClient();
        await httpClient.CreateClientAsync(new(newClient), cancellationToken);
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

    private async Task CreateInvoiceInternalAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var inFaktInvoice = invoice.ToInFaktInvoice();
        await httpClient.CreateInvoiceAsync(new(inFaktInvoice), cancellationToken);
    }
}
