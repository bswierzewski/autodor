using Autodor.Shared.Contracts.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;
    private readonly IProductsAPI _productsApi;

    public CreateInvoiceCommandHandler(
        IMediator mediator,
        ILogger<CreateInvoiceCommandHandler> logger,
        IProductsAPI productsApi)
    {
        _mediator = mediator;
        _logger = logger;
        _productsApi = productsApi;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating invoice for contractor {ContractorId} with {OrderCount} orders",
            request.ContractorId, request.OrderNumbers.Count());

        // PRZYKŁAD UŻYCIA API: Pobieranie konkretnego produktu
        const string specificPartNumber = "007935016720";

        _logger.LogInformation("Fetching product details for part number: {PartNumber}", specificPartNumber);

        var productDetails = await _productsApi.GetProductByNumberAsync(specificPartNumber, cancellationToken);

        if (productDetails != null)
        {
            _logger.LogInformation("Found product: {Name} (Number: {Number}, EAN13: {EAN13})",
                productDetails.Name,
                productDetails.Number,
                productDetails.EAN13);
        }
        else
        {
            _logger.LogWarning("Product {PartNumber} not found in Products module", specificPartNumber);
        }

        // Tymczasowo zwracamy nowy GUID, docelowo będzie to ID utworzonej faktury
        var invoiceId = Guid.NewGuid();

        _logger.LogInformation("Invoice created with ID: {InvoiceId}", invoiceId);

        return invoiceId;

        // TODO: Pełna implementacja tworzenia faktury
        // throw new NotImplementedException("Full invoice creation is not implemented yet.");

        //// Pobierz dane kontrahenta z modułu Contractors
        //var contractor = await _mediator.Send(new GetContractorQuery(request.ContractorId), cancellationToken);
        //if (contractor == null)
        //{
        //    throw new InvalidOperationException($"Contractor with ID {request.ContractorId} not found");
        //}

        //// Pobierz zamówienia z modułu Orders
        //var orders = await _mediator.Send(new GetOrdersByIdsQuery(request.OrderNumbers), cancellationToken);
        //if (!orders.Any())
        //{
        //    throw new InvalidOperationException("No valid orders found for the provided order numbers");
        //}

        //// Pobierz wszystkie unikalne numery części z zamówień
        //var partNumbers = orders
        //    .SelectMany(o => o.Items.Select(i => i.PartNumber))
        //    .Distinct()
        //    .ToList();

        //// Pobierz dane produktów z modułu Products
        //var products = await _mediator.Send(new GetProductsQuery(partNumbers), cancellationToken);
        //var productDict = products.ToDictionary(p => p.PartNumber);

        //// Tworzenie faktury
        //var invoiceNumber = GenerateInvoiceNumber();
        //var invoice = new Invoice(invoiceNumber, DateTime.Now, request.ContractorId);

        //// Dodawanie pozycji faktury na podstawie zamówień
        //foreach (var order in orders)
        //{
        //    foreach (var orderItem in order.Items)
        //    {
        //        if (productDict.TryGetValue(orderItem.PartNumber, out var product))
        //        {
        //            var invoiceItem = new InvoiceItem(
        //                new InvoiceItemId(Guid.NewGuid()),
        //                orderItem.PartNumber,
        //                product.Name,
        //                orderItem.Quantity,
        //                orderItem.UnitPrice
        //            );
        //            invoice.AddItem(invoiceItem);
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Product {PartNumber} not found for order item", orderItem.PartNumber);
        //        }
        //    }
        //}

        //if (!invoice.Items.Any())
        //{
        //    throw new InvalidOperationException("No valid invoice items could be created");
        //}

        //// Tutaj normalnie byłoby zapisywanie do bazy danych przez Repository/UnitOfWork
        //// Na razie symulujemy zapisanie
        //_logger.LogInformation("Invoice {InvoiceNumber} created with {ItemCount} items, total amount: {TotalAmount:C}", 
        //    invoice.InvoiceNumber, invoice.Items.Count, invoice.TotalAmount);

        //// Wysłanie eventu o utworzeniu faktury
        //var invoiceCreatedEvent = new InvoiceCreatedEvent(
        //    invoice.Id.Value,
        //    invoice.InvoiceNumber,
        //    request.OrderNumbers,
        //    DateTime.UtcNow
        //);

        //await _mediator.Publish(invoiceCreatedEvent, cancellationToken);

        //return invoice.Id.Value;
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{DateTime.Now.Ticks % 10000:D4}";
    }
}