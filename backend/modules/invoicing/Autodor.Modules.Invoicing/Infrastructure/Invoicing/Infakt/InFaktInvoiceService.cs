using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Filters;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Extensions;
using InFaktClient = Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses.Client;
using InvoiceProcessingResponse = Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses.InvoiceProcessingResponse;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt;

public class InFaktInvoiceService(IInFaktHttpClient httpClient) : IInvoiceService
{
    // InFakt API finall status codes for invoice processing
    private const int InvoiceCreatedCode = 201;
    private const int InvoiceCreationFailedCode = 422;

    // Infakt API processing codes for invoice processing (pending)
    private const int ProcessingQueuedCode = 100;
    private const int ProcessingWaitingCode = 120;
    private const int ProcessingInProgressCode = 140;

    private const int MaxStatusChecks = 30;
    private static readonly TimeSpan StatusCheckDelay = TimeSpan.FromSeconds(1);

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
        var processingResponse = await httpClient.CreateInvoiceAsync(
            new(invoice.ToInFaktInvoice()),
            cancellationToken);

        if (IsPending(processingResponse.ProcessingCode))
        {
            var taskReferenceNumber = !string.IsNullOrWhiteSpace(processingResponse.TaskReferenceNumber)
                ? processingResponse.TaskReferenceNumber
                : throw new InvalidOperationException("InFakt nie zwrócił numeru referencyjnego zadania tworzenia faktury.");

            processingResponse = await WaitForInvoiceProcessingAsync(taskReferenceNumber, cancellationToken);
        }

        EnsureSuccess(processingResponse);
    }

    private async Task<InvoiceProcessingResponse> WaitForInvoiceProcessingAsync(
        string taskReferenceNumber,
        CancellationToken cancellationToken)
    {
        for (var check = 1; check <= MaxStatusChecks; check++)
        {
            var response = await httpClient.GetInvoiceProcessingStatusAsync(
                taskReferenceNumber,
                cancellationToken);

            if (!IsPending(response.ProcessingCode))
                return response;

            if (check < MaxStatusChecks)
                await Task.Delay(StatusCheckDelay, cancellationToken);
        }

        throw new TimeoutException(
            $"InFakt nie zakończył tworzenia faktury po {MaxStatusChecks} sprawdzeniach. " +
            $"Zadanie: {taskReferenceNumber}.");
    }

    private static void EnsureSuccess(InvoiceProcessingResponse response)
    {
        switch (response.ProcessingCode)
        {
            case InvoiceCreatedCode:
                return;

            case InvoiceCreationFailedCode:
                throw new InvalidOperationException(
                    $"InFakt odrzucił fakturę. Powód: {ExtractErrorDetails(response)}");

            case var processingCode when IsPending(processingCode):
                throw new TimeoutException(
                    $"InFakt nie zakończył tworzenia faktury po {MaxStatusChecks} próbach. " +
                    $"Zadanie: {response.TaskReferenceNumber}.");

            default:
                throw new InvalidOperationException(
                    $"InFakt zwrócił nieznany status: " +
                    $"{response.ProcessingDescription ?? "brak opisu"} " +
                    $"(kod: {response.ProcessingCode}).");
        }
    }

    private static bool IsPending(int processingCode) =>
        processingCode is ProcessingQueuedCode or ProcessingWaitingCode or ProcessingInProgressCode;

    private static string ExtractErrorDetails(InvoiceProcessingResponse response)
    {
        if (response.InvoiceErrors is { Count: > 0 })
        {
            var errors = response.InvoiceErrors.SelectMany(error =>
                error.Value.Select(message => $"{error.Key}: {message}"));

            return string.Join("; ", errors);
        }

        return response.ProcessingDescription ?? "Nieznany błąd walidacji.";
    }
}
