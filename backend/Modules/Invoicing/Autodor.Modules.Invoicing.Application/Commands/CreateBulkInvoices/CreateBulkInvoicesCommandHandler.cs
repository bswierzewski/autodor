using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Domain.Entities;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

public class CreateBulkInvoicesCommandHandler : IRequestHandler<CreateBulkInvoicesCommand, IEnumerable<Guid>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateBulkInvoicesCommandHandler> _logger;

    public CreateBulkInvoicesCommandHandler(IMediator mediator, ILogger<CreateBulkInvoicesCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IEnumerable<Guid>> Handle(CreateBulkInvoicesCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //_logger.LogInformation("Creating bulk invoices for contractor {ContractorId} from {DateFrom} to {DateTo}", 
        //    request.ContractorId, request.DateFrom, request.DateTo);

        //// Pobierz dane kontrahenta z modułu Contractors
        //var contractor = await _mediator.Send(new GetContractorQuery(request.ContractorId), cancellationToken);
        //if (contractor == null)
        //{
        //    throw new InvalidOperationException($"Contractor with ID {request.ContractorId} not found");
        //}

        //// Pobierz wszystkie zamówienia kontrahenta z podanego okresu z modułu Orders
        //var ordersInPeriod = await _mediator.Send(new GetOrdersByContractorAndPeriodQuery(
        //    request.ContractorId, request.DateFrom, request.DateTo), cancellationToken);

        //if (!ordersInPeriod.Any())
        //{
        //    _logger.LogWarning("No orders found for contractor {ContractorId} in period {DateFrom} to {DateTo}", 
        //        request.ContractorId, request.DateFrom, request.DateTo);
        //    return Enumerable.Empty<Guid>();
        //}

        //_logger.LogInformation("Found {OrderCount} orders for bulk invoicing", ordersInPeriod.Count());

        //// Pobierz wszystkie unikalne numery części z zamówień
        //var partNumbers = ordersInPeriod
        //    .SelectMany(o => o.Items.Select(i => i.PartNumber))
        //    .Distinct()
        //    .ToList();

        //// Pobierz dane produktów z modułu Products
        //var products = await _mediator.Send(new GetProductsQuery(partNumbers), cancellationToken);
        //var productDict = products.ToDictionary(p => p.PartNumber);

        //var createdInvoiceIds = new List<Guid>();

        //// Tworzenie osobnej faktury dla każdego zamówienia
        //foreach (var order in ordersInPeriod)
        //{
        //    try
        //    {
        //        var invoiceNumber = GenerateInvoiceNumber();
        //        var invoice = new Invoice(invoiceNumber, DateTime.Now, request.ContractorId);

        //        // Dodawanie pozycji faktury na podstawie zamówienia
        //        foreach (var orderItem in order.Items)
        //        {
        //            if (productDict.TryGetValue(orderItem.PartNumber, out var product))
        //            {
        //                var invoiceItem = new InvoiceItem(
        //                    new InvoiceItemId(Guid.NewGuid()),
        //                    orderItem.PartNumber,
        //                    product.Name,
        //                    orderItem.Quantity,
        //                    orderItem.UnitPrice
        //                );
        //                invoice.AddItem(invoiceItem);
        //            }
        //            else
        //            {
        //                _logger.LogWarning("Product {PartNumber} not found for order {OrderNumber}", 
        //                    orderItem.PartNumber, order.OrderNumber);
        //            }
        //        }

        //        if (invoice.Items.Any())
        //        {
        //            // Tutaj normalnie byłoby zapisywanie do bazy danych przez Repository/UnitOfWork
        //            // Na razie symulujemy zapisanie
        //            _logger.LogInformation("Invoice {InvoiceNumber} created for order {OrderNumber} with {ItemCount} items, total amount: {TotalAmount:C}", 
        //                invoice.InvoiceNumber, order.OrderNumber, invoice.Items.Count, invoice.TotalAmount);

        //            createdInvoiceIds.Add(invoice.Id.Value);

        //            // Wysłanie eventu o utworzeniu faktury
        //            var invoiceCreatedEvent = new InvoiceCreatedEvent(
        //                invoice.Id.Value,
        //                invoice.InvoiceNumber,
        //                new[] { order.OrderNumber },
        //                DateTime.UtcNow
        //            );

        //            await _mediator.Publish(invoiceCreatedEvent, cancellationToken);
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Skipping invoice creation for order {OrderNumber} - no valid items found", order.OrderNumber);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating invoice for order {OrderNumber}", order.OrderNumber);
        //    }
        //}

        //_logger.LogInformation("Bulk invoicing completed. Created {InvoiceCount} invoices out of {OrderCount} orders", 
        //    createdInvoiceIds.Count, ordersInPeriod.Count());

        //return createdInvoiceIds;
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now.Ticks % 10000:D4}";
    }
}