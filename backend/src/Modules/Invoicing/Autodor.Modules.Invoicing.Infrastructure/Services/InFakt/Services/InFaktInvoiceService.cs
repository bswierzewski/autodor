using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Services;

public class InFaktInvoiceService(InFaktHttpClient httpClient) : IInvoiceService
{
    public async Task<bool> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        // Upsert client (ensure it exists in InFakt system and is up to date)
        await UpsertClientAsync(invoice.Contractor, cancellationToken);

        // Create invoice (InFakt will automatically link to client by NIP)
        await CreateInvoiceInternalAsync(invoice, cancellationToken);

        return true;
    }

    private async Task UpsertClientAsync(Contractor contractor, CancellationToken cancellationToken)
    {
        // Search for existing client by NIP
        var searchQuery = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter
            {
                NipEq = contractor.NIP
            }
        };

        var clientList = await httpClient.GetClientsAsync(searchQuery, cancellationToken);

        // If client exists, check if update is needed
        if (clientList.Entities.Count != 0)
        {
            var existingClient = clientList.Entities.First();
            if (existingClient.Id.HasValue)
            {
                // Check if client data has changed
                if (RequiresUpdate(existingClient, contractor))
                {
                    var updatedClient = contractor.ToInFaktClient();
                    await httpClient.UpdateClientAsync(existingClient.Id.Value, updatedClient, cancellationToken);
                }

                return;
            }
        }

        // Client doesn't exist, create new one
        var newClient = contractor.ToInFaktClient();
        await httpClient.CreateClientAsync(newClient, cancellationToken);
    }

    private static bool RequiresUpdate(
        Clients.Models.Responses.Client existingClient,
        Contractor contractor)
    {
        // Compare company name
        if (!string.Equals(existingClient.CompanyName, contractor.Name, StringComparison.OrdinalIgnoreCase))
            return true;

        // Compare street address
        if (!string.Equals(existingClient.Street, contractor.Street, StringComparison.OrdinalIgnoreCase))
            return true;

        // Compare city
        if (!string.Equals(existingClient.City, contractor.City, StringComparison.OrdinalIgnoreCase))
            return true;

        // Compare postal code
        if (!string.Equals(existingClient.PostalCode, contractor.ZipCode, StringComparison.OrdinalIgnoreCase))
            return true;

        // Compare NIP (should be the same since we searched by NIP, but check anyway)
        if (!string.Equals(existingClient.Nip, contractor.NIP, StringComparison.OrdinalIgnoreCase))
            return true;

        // Compare email
        if (!string.Equals(existingClient.Email, contractor.Email, StringComparison.OrdinalIgnoreCase))
            return true;

        // No changes detected
        return false;
    }

    private async Task CreateInvoiceInternalAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var inFaktInvoice = invoice.ToInFaktInvoice();

        await httpClient.CreateInvoiceAsync(inFaktInvoice, cancellationToken);
    }
}
