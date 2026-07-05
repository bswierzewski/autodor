using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;
using BuildingBlocks.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateInvoices;

public static class CreateInvoicesHandler
{
    public static async Task<CreateInvoicesResult> Handle(
        CreateInvoicesCommand command,
        IMessageBus bus,
        IServiceProvider serviceProvider,
        IOptions<InvoicingOptions> options,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(typeof(CreateInvoicesHandler));

        logger.LogInformation("Creating bulk invoices for date range {DateFrom} to {DateTo}", command.DateFrom, command.DateTo);

        // Fetch all orders that fall within the requested billing period.
        // The query crosses module boundaries via the Wolverine message bus.
        var orders = (await bus.InvokeAsync<IEnumerable<OrderDto>>(
            new GetOrdersByDateRangeQuery(command.DateFrom, command.DateTo), ct)).ToList();

        if (orders.Count == 0)
        {
            logger.LogWarning("No orders found for bulk invoice creation in date range {DateFrom} to {DateTo}", command.DateFrom, command.DateTo);
            throw new NotFoundException("Nie znaleziono zamówień do zbiorczego tworzenia faktur.");
        }

        // Group orders by the contractor's NIP (tax ID) so that each contractor
        // receives exactly one consolidated invoice covering all their orders in the period.
        // Orders without a CustomerNumber are silently skipped — they cannot be matched
        // to a contractor and therefore cannot be invoiced.
        var ordersByContractor = orders
            .Where(o => !string.IsNullOrWhiteSpace(o.CustomerNumber))
            .GroupBy(o => o.CustomerNumber)
            .ToList();

        // Resolve contractor details for all NIPs in a single cross-module query
        // instead of querying one by one inside the loop (avoids N+1 calls).
        var contractors = await bus.InvokeAsync<IEnumerable<ContractorDto>>(
            new GetContractorsByNIPsQuery([.. ordersByContractor.Select(g => g.Key)]), ct);

        // Index contractors by NIP for O(1) look-ups inside ProcessContractorAsync.
        var contractorDict = contractors.ToDictionary(c => c.NIP, c => c);

        // Resolve the correct invoice provider (InFakt / iFirma) based on configuration.
        // The keyed service allows switching providers without changing handler logic.
        var invoiceService = serviceProvider.GetRequiredKeyedService<IInvoiceService>(options.Value.Provider);

        // Process each contractor sequentially to stay within external API rate limits.
        // Each result records whether the invoice was created or failed, so a single
        // contractor error does not abort the entire batch.
        var details = new List<InvoiceSummaryEntry>(ordersByContractor.Count);
        foreach (var group in ordersByContractor)
            details.Add(await ProcessContractorAsync(group, contractorDict, invoiceService, command.DateTo, logger, ct));

        var result = new CreateInvoicesResult(details.AsReadOnly());

        logger.LogInformation("Bulk invoicing complete: {Created} created, {Skipped} failed",
            result.InvoicesCreated, result.InvoicesSkipped);

        return result;
    }

    /// <summary>
    /// Builds and submits a single consolidated invoice for one contractor group.
    /// Returns an <see cref="InvoiceSummaryEntry"/> regardless of success or failure
    /// so the caller can continue processing the remaining contractors.
    /// </summary>
    private static async Task<InvoiceSummaryEntry> ProcessContractorAsync(
        IGrouping<string, OrderDto> group,
        Dictionary<string, ContractorDto> contractorDict,
        IInvoiceService invoiceService,
        DateTime saleDate,
        ILogger logger,
        CancellationToken ct)
    {
        var nip = group.Key;

        // If the contractor module returned no record for this NIP, record the failure
        // and continue — the scheduler / operator will see the gap in the summary email.
        if (!contractorDict.TryGetValue(nip, out var contractorDto))
        {
            logger.LogWarning("Contractor with NIP {NIP} not found, skipping", nip);
            return new InvoiceSummaryEntry(nip, "(nieznany)", 0, false, "Nie znaleziono kontrahenta");
        }

        // Flatten all line items across every order for this contractor.
        // Prices are rounded to 2 decimal places to match external API expectations.
        var items = group
            .SelectMany(o => o.Items)
            .Select(i => new InvoiceItem { Name = i.Name, Quantity = i.Quantity, UnitPrice = Math.Round(i.Price, 2) })
            .ToList();

        var invoice = new Invoice
        {
            Number = null,                              // assigned by the external provider
            IssueDate = DateTime.Today,
            SaleDate = saleDate,                        // last day of the billing period
            PaymentDue = DateTime.Today.AddDays(14),
            Contractor = new Contractor(contractorDto.Name, contractorDto.City, contractorDto.Street,
                contractorDto.NIP, contractorDto.ZipCode, contractorDto.Email),
            Items = items.AsReadOnly()
        };

        try
        {
            await invoiceService.CreateInvoiceAsync(invoice, ct);
            logger.LogInformation("Created invoice for {NIP} ({Name}) with {Count} items", nip, contractorDto.Name, items.Count);
            return new InvoiceSummaryEntry(nip, contractorDto.Name, items.Count, true, null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Propagate cancellation immediately — do not swallow it as a regular failure.
            throw;
        }
        catch (Exception ex)
        {
            // Record the error and let the batch continue with the next contractor.
            logger.LogError(ex, "Failed to create invoice for {NIP} ({Name})", nip, contractorDto.Name);
            return new InvoiceSummaryEntry(nip, contractorDto.Name, items.Count, false, ex.Message);
        }
    }
}
