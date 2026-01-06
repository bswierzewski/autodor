using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Models.Filters;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Services;

public class InFaktInvoiceService(InFaktHttpClient httpClient) : IInvoiceService
{
    public async Task<ErrorOr<bool>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        // Upsert client (ensure it exists in InFakt system and is up to date)
        var upsertResult = await UpsertClientAsync(invoice.Contractor, cancellationToken);
        if (upsertResult.IsError)
            return upsertResult.Errors;

        // Create invoice (InFakt will automatically link to client by NIP)
        var createResult = await CreateInvoiceInternalAsync(invoice, cancellationToken);
        if (createResult.IsError)
            return createResult.Errors;

        return true;
    }

    private async Task<ErrorOr<bool>> UpsertClientAsync(Contractor contractor, CancellationToken cancellationToken)
    {
        // Search for existing client by NIP
        var searchQuery = new ClientSearchQuery
        {
            Filter = new ClientSearchFilter
            {
                NipEq = contractor.NIP
            }
        };

        var clientListResult = await httpClient.GetClientsAsync(searchQuery, cancellationToken);

        if (clientListResult.IsError)
            return clientListResult.Errors;

        // If client exists, check if update is needed
        if (clientListResult.Value.Entities.Count != 0)
        {
            var existingClient = clientListResult.Value.Entities.First();
            if (existingClient.Id.HasValue)
            {
                // Check if client data has changed
                if (RequiresUpdate(existingClient, contractor))
                {
                    var updatedClient = contractor.ToInFaktClient();
                    var updateResult = await httpClient.UpdateClientAsync(existingClient.Id.Value, updatedClient, cancellationToken);

                    if (updateResult.IsError)
                        return updateResult.Errors;
                }

                return true;
            }
        }

        // Client doesn't exist, create new one
        var newClient = contractor.ToInFaktClient();
        var createResult = await httpClient.CreateClientAsync(newClient, cancellationToken);

        if (createResult.IsError)
            return createResult.Errors;

        return true;
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

    private async Task<ErrorOr<bool>> CreateInvoiceInternalAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var inFaktInvoice = invoice.ToInFaktInvoice();

        var createResult = await httpClient.CreateInvoiceAsync(inFaktInvoice, cancellationToken);

        if (createResult.IsError)
            return createResult.Errors;

        return true;
    }
}
