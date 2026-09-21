using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Autodor.Modules.Invoicing.Features.CreateManualInvoice;

public static class CreateManualInvoiceHandler
{
    [Authorize]
    public static async Task<IResult> Handle(
        CreateManualInvoiceCommand command,
        IMessageBus bus,
        IInvoiceService invoiceService,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(typeof(CreateManualInvoiceHandler));

        logger.LogInformation(
            "Creating manual invoice for contractor {ContractorNIP} with {ItemCount} items",
            command.ContractorNIP,
            command.Items.Count);

        var contractor = await bus.InvokeAsync<ContractorDto?>(
            new GetContractorByNIPQuery(command.ContractorNIP), ct);

        if (contractor == null)
        {
            logger.LogError("Contractor with NIP {ContractorNIP} not found", command.ContractorNIP);
            throw new NotFoundException($"Nie znaleziono kontrahenta o numerze NIP {command.ContractorNIP}.");
        }

        var invoiceItems = command.Items
            .Select(item => new InvoiceItem
            {
                Name = item.ItemNumber.Trim(),
                Quantity = item.Quantity,
                UnitPrice = Math.Round(item.NetUnitPrice, 2)
            })
            .ToList()
            .AsReadOnly();

        var invoice = InvoiceFactory.Create(
            command.InvoiceNumber,
            command.IssueDate,
            command.SaleDate,
            contractor,
            invoiceItems);

        await invoiceService.CreateInvoiceAsync(invoice, ct);

        logger.LogInformation("Successfully created manual invoice for contractor {ContractorNIP}", command.ContractorNIP);

        return Results.Ok();
    }
}
