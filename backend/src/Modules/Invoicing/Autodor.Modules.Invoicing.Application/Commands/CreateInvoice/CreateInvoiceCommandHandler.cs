using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

/// <summary>
/// Handles the creation of invoices by aggregating order data, enriching with product details,
/// and sending to external invoicing systems.
/// </summary>
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;
    private readonly IProductsAPI _productsApi;
    private readonly IOrdersAPI _ordersApi;
    private readonly IContractorsAPI _contractorsApi;
    private readonly IInvoiceServiceFactory _invoiceServiceFactory;

    public CreateInvoiceCommandHandler(
        ILogger<CreateInvoiceCommandHandler> logger,
        IProductsAPI productsApi,
        IOrdersAPI ordersApi,
        IContractorsAPI contractorsApi,
        IInvoiceServiceFactory invoiceServiceFactory)
    {
        _logger = logger;
        _productsApi = productsApi;
        _ordersApi = ordersApi;
        _contractorsApi = contractorsApi;
        _invoiceServiceFactory = invoiceServiceFactory;
    }

    /// <summary>
    /// Creates an invoice by retrieving orders, enriching them with product data, and submitting to external invoicing system.
    /// </summary>
    /// <param name="request">Invoice creation parameters including dates, order IDs, and contractor information</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Unique identifier of the created invoice from the external system</returns>
    /// <exception cref="InvalidOperationException">Thrown when no orders are found or contractor doesn't exist</exception>
    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating invoice for contractor {ContractorId} with {OrderCount} orders from {DateCount} dates",
            request.ContractorId, request.OrderIds.Count(), request.Dates.Count());

        // Retrieve all orders from specified dates
        var orders = await _ordersApi.GetOrdersByDatesAsync(request.Dates, cancellationToken);

        // Filter orders to include only those matching the provided order IDs
        var filteredOrders = orders.Where(o => request.OrderIds.Contains(o.Id)).ToList();

        if (filteredOrders.Count == 0)
        {
            _logger.LogWarning("No orders found for the specified order IDs");
            throw new InvalidOperationException("No orders found for the specified order IDs");
        }

        // Extract unique product numbers from all order items for bulk product lookup
        var productNumbers = filteredOrders
            .SelectMany(o => o.Items)
            .Select(i => i.Number)
            .Distinct()
            .ToList();

        var products = await _productsApi.GetProductsByNumbersAsync(productNumbers, cancellationToken);
        var productDict = products.ToDictionary(p => p.Number, p => p);

        // Get contractor details for invoice generation
        var contractor = await _contractorsApi.GetContractorByIdAsync(request.ContractorId, cancellationToken);
        if (contractor == null)
        {
            _logger.LogError("Contractor with ID {ContractorId} not found", request.ContractorId);
            throw new InvalidOperationException($"Contractor with ID {request.ContractorId} not found");
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

        // Enrich order items with product names, showing format "Product Name (PartNumber)" when available
        var invoiceItems = filteredOrders
            .SelectMany(o => o.Items)
            .Select(item =>
            {
                var partNumber = item.Number ?? string.Empty;
                var productName = productDict.TryGetValue(partNumber, out var product)
                    ? $"{product.Name} ({partNumber})"
                    : partNumber;

                return new InvoiceItem
                {
                    Name = productName,
                    Quantity = item.Quantity,
                    UnitPrice = Math.Round(item.Price, 2)
                };
            })
            .ToList();

        var invoice = new Invoice
        {
            Number = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            SaleDate = request.SaleDate,
            PaymentDue = request.IssueDate.AddDays(14), // Standard 14-day payment terms
            Contractor = invoiceContractor,
            Items = invoiceItems.AsReadOnly()
        };

        var invoiceService = _invoiceServiceFactory.GetInvoiceService();
        var invoiceId = await invoiceService.CreateInvoiceAsync(invoice, cancellationToken);

        _logger.LogInformation("Successfully created invoice with ID {InvoiceId}", invoiceId);

        return invoiceId;
    }
}