using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

/// <summary>
/// Handles the creation of multiple invoices by processing orders within a date range,
/// excluding specified orders, and grouping by contractor for batch invoice generation.
/// </summary>
public class CreateBulkInvoicesCommandHandler : IRequestHandler<CreateBulkInvoicesCommand, IEnumerable<string>>
{
    private readonly ILogger<CreateBulkInvoicesCommandHandler> _logger;
    private readonly IProductsAPI _productsApi;
    private readonly IOrdersAPI _ordersApi;
    private readonly IContractorsAPI _contractorsApi;
    private readonly IInvoiceServiceFactory _invoiceServiceFactory;

    public CreateBulkInvoicesCommandHandler(
        ILogger<CreateBulkInvoicesCommandHandler> logger,
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
    /// Creates multiple invoices by processing all orders in date range, excluding specified orders,
    /// and grouping remaining orders by contractor for individual invoice generation.
    /// </summary>
    /// <param name="request">Bulk invoice creation parameters including date range</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Collection of unique identifiers for all created invoices</returns>
    /// <exception cref="InvalidOperationException">Thrown when no orders are found in the date range</exception>
    public async Task<IEnumerable<string>> Handle(CreateBulkInvoicesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating bulk invoices for date range {DateFrom} to {DateTo}",
            request.DateFrom, request.DateTo);

        // Retrieve all orders within the specified date range
        var orders = await _ordersApi.GetOrdersByDateRangeAsync(request.DateFrom, request.DateTo, cancellationToken);

        // Get excluded order IDs to filter them out
        var excludedOrderIds = await _ordersApi.GetExcludedOrderIdsAsync(cancellationToken);
        var excludedSet = excludedOrderIds.ToHashSet();

        // Filter out excluded orders
        var validOrders = orders.Where(o => !excludedSet.Contains(o.Id)).ToList();

        if (validOrders.Count == 0)
        {
            _logger.LogWarning("No valid orders found for bulk invoice creation in date range {DateFrom} to {DateTo}",
                request.DateFrom, request.DateTo);
            throw new InvalidOperationException("No valid orders found for bulk invoice creation");
        }

        _logger.LogInformation("Found {ValidOrderCount} valid orders after excluding {ExcludedCount} excluded orders",
            validOrders.Count, excludedOrderIds.Count());

        // Extract unique product numbers from all order items for bulk product lookup
        var productNumbers = validOrders
            .SelectMany(o => o.Items)
            .Select(i => i.Number)
            .Distinct()
            .ToList();

        var products = await _productsApi.GetProductsByNumbersAsync(productNumbers, cancellationToken);
        var productDict = products.ToDictionary(p => p.Number, p => p);

        // Get unique contractor NIPs from orders for bulk contractor lookup
        var contractorNIPs = validOrders
            .Select(o => o.Contractor.Number) // Order contractor number is the NIP
            .Distinct()
            .ToList();

        var contractors = await _contractorsApi.GetContractorsByNIPsAsync(contractorNIPs, cancellationToken);
        var contractorDict = contractors.ToDictionary(c => c.NIP, c => c);

        // Group orders by contractor NIP for individual invoice creation
        var ordersByContractor = validOrders
            .GroupBy(o => o.Contractor.Number) // Group by contractor number (which is NIP)
            .ToList();

        _logger.LogInformation("Grouped orders into {ContractorCount} contractors", ordersByContractor.Count);

        var invoiceNumbers = new List<string>();

        foreach (var contractorGroup in ordersByContractor)
        {
            var contractorOrders = contractorGroup.ToList();
            var contractorNIP = contractorGroup.Key; // This is actually the NIP

            _logger.LogInformation("Creating invoice for contractor NIP {ContractorNIP} with {OrderCount} orders",
                contractorNIP, contractorOrders.Count);

            try
            {
                // Get contractor details from the pre-loaded dictionary
                if (!contractorDict.TryGetValue(contractorNIP, out var contractorDto))
                {
                    _logger.LogWarning("Contractor with NIP {ContractorNIP} not found, skipping orders", contractorNIP);
                    continue;
                }

                // Map contractor DTO to domain value object
                var invoiceContractor = new Contractor(
                    Name: contractorDto.Name,
                    City: contractorDto.City,
                    Street: contractorDto.Street,
                    NIP: contractorDto.NIP,
                    ZipCode: contractorDto.ZipCode,
                    Email: contractorDto.Email
                );

                // Enrich order items with product names
                var invoiceItems = contractorOrders
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

                // Create invoice using the current date as both issue and sale date
                var invoice = new Invoice
                {
                    Number = null, // Let external system generate number
                    IssueDate = DateTime.Today,
                    SaleDate = DateTime.Today,
                    PaymentDue = DateTime.Today.AddDays(14), // Standard 14-day payment terms
                    Contractor = invoiceContractor,
                    Items = invoiceItems.AsReadOnly()
                };

                // Execute pre-processing if required by the current provider
                if (_invoiceServiceFactory.GetInvoicePreProcessor() is IInvoicePreProcessor preProcessor)
                    await preProcessor.PrepareInvoiceAsync(invoice, cancellationToken);                

                var invoiceService = _invoiceServiceFactory.GetInvoiceService();
                var invoiceNumber = await invoiceService.CreateInvoiceAsync(invoice, cancellationToken);

                invoiceNumbers.Add(invoiceNumber);

                _logger.LogInformation("Successfully created invoice {InvoiceNumber} for contractor {ContractorNIP}",
                    invoiceNumber, contractorNIP);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create invoice for contractor {ContractorNIP}",
                    contractorNIP);
                throw;
            }
        }

        _logger.LogInformation("Successfully created {InvoiceCount} bulk invoices", invoiceNumbers.Count);
        return invoiceNumbers;
    }
}