using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Products;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler(
    ILogger<CreateInvoiceCommandHandler> logger,
    IProductsAPI productsApi,
    IOrdersAPI ordersApi,
    IContractorsAPI contractorsApi,
    IServiceProvider serviceProvider,
    IOptions<InvoicingOptions> options) : IRequestHandler<CreateInvoiceCommand, Unit>
{
    public async Task<Unit> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating invoice for contractor {ContractorId} with {OrderCount} orders from {DateCount} dates",
            request.ContractorId, request.OrderIds.Count(), request.Dates.Count());

        // Retrieve all orders from specified dates
        var orders = await ordersApi.GetOrdersByDatesAsync(request.Dates, cancellationToken);

        // Filter orders to include only those matching the provided order IDs
        var filteredOrders = orders.Where(o => request.OrderIds.Contains(o.Id)).ToList();

        if (filteredOrders.Count == 0)
        {
            logger.LogWarning("No orders found for the specified order IDs");
            throw new InvalidOperationException("No orders found for the specified order IDs");
        }

        // Extract unique product numbers from all order items for bulk product lookup
        var productNumbers = filteredOrders
            .SelectMany(o => o.Items)
            .Select(i => i.Number)
            .Distinct()
            .ToList();

        var products = await productsApi.GetProductsByNumbersAsync(productNumbers, cancellationToken);
        var productDict = products.ToDictionary(p => p.Number, p => p);

        // Get contractor details for invoice generation
        var contractor = await contractorsApi.GetContractorByIdAsync(request.ContractorId, cancellationToken);
        if (contractor == null)
        {
            logger.LogError("Contractor with ID {ContractorId} not found", request.ContractorId);
            throw new KeyNotFoundException($"Contractor with ID {request.ContractorId} not found");
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

        var invoiceService = serviceProvider.GetRequiredKeyedService<IInvoiceService>(options.Value.Provider);

        try
        {
            await invoiceService.CreateInvoiceAsync(invoice, cancellationToken);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create invoice: {Error}", ex.Message);
            throw;
        }
    }
}