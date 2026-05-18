using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;
using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wolverine;
using Wolverine.Http;

namespace Autodor.Modules.Invoicing.Features.CreateInvoice;

public class CreateInvoiceHandler
{
    [WolverinePost("/api/invoices")]
    [Tags("Invoicing")]
    [EndpointName("CreateInvoice")]
    [EndpointSummary("Create a single invoice for selected orders")]
    public static async Task<IResult> Handle(
        CreateInvoiceCommand command,
        IMessageBus bus,
        IServiceProvider serviceProvider,
        IOptions<InvoicingOptions> options,
        ILogger<CreateInvoiceHandler> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Creating invoice for contractor {ContractorNip} with {OrderCount} order IDs from {DateCount} dates",
            command.ContractorNip, command.OrderIds.Count(), command.Dates.Count());

        // Retrieve all orders from specified dates (already filtered for exclusions by Orders module)
        var orders = await bus.InvokeAsync<IEnumerable<OrderDto>>(new GetOrdersByDatesQuery(command.Dates), ct);

        // Filter orders to include only those matching the provided order IDs
        var orderIdSet = command.OrderIds.ToHashSet();
        var filteredOrders = orders.Where(o => orderIdSet.Contains(o.Id)).ToList();

        if (filteredOrders.Count == 0)
        {
            logger.LogWarning("No orders found for the specified order IDs");
            throw new NotFoundException("No orders found for the specified order IDs");
        }

        // Get contractor details for invoice generation
        var contractor = await bus.InvokeAsync<ContractorDto?>(
            new GetContractorByNipQuery(command.ContractorNip), ct);

        if (contractor == null)
        {
            logger.LogError("Contractor with NIP {ContractorNip} not found", command.ContractorNip);
            throw new NotFoundException($"Contractor with NIP {command.ContractorNip} was not found");
        }

        // Map contractor DTO to domain value object
        var invoiceContractor = new Contractor(
            Name: contractor.Name,
            City: contractor.City,
            Street: contractor.Street,
            NIP: contractor.NIP,
            ZipCode: contractor.ZipCode,
            Email: contractor.Email
        );

        // Collect all order items (already filtered for exclusions by Orders module)
        var invoiceItems = filteredOrders
            .SelectMany(o => o.Items)
            .Select(item => new InvoiceItem
            {
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = Math.Round(item.Price, 2)
            })
            .ToList();

        var invoice = new Invoice
        {
            Number = command.InvoiceNumber,
            IssueDate = command.IssueDate,
            SaleDate = command.SaleDate,
            PaymentDue = command.IssueDate.AddDays(14), // Standard 14-day payment terms
            Contractor = invoiceContractor,
            Items = invoiceItems.AsReadOnly()
        };

        var invoiceService = serviceProvider.GetRequiredKeyedService<IInvoiceService>(options.Value.Provider);

        await invoiceService.CreateInvoiceAsync(invoice, ct);

        logger.LogInformation("Successfully created invoice for contractor {ContractorNip}", command.ContractorNip);

        return Results.Ok();
    }
}
