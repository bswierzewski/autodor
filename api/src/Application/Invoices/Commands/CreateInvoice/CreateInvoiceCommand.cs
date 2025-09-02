using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommand : IRequest<Result<string>>
{
    public int? InvoiceNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public DateTime IssueDate { get; set; }
    public IEnumerable<DateTime> Dates { get; set; }
    public IEnumerable<string> OrderIds { get; set; }
    public int ContractorId { get; set; }
}

public class CreateInvoiceCommandHandler(
    IInvoiceService invoiceService,
    IDistributorsSalesService distributorsSalesService,
    IProductsService productsService,
    IApplicationDbContext context) : IRequestHandler<CreateInvoiceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Start both tasks in parallel
        var ordersTask = FetchOrdersAsync(request.Dates);
        var productsTask = productsService.GetProductsAsync();

        await Task.WhenAll(ordersTask, productsTask).ConfigureAwait(false);

        var allOrders = await ordersTask.ConfigureAwait(false);
        var products = await productsTask.ConfigureAwait(false);

        var filteredOrders = FilterOrdersByIds(allOrders, request.OrderIds);
        var items = MapInvoiceItems(filteredOrders, products);
        var invoice = await CreateInvoiceAsync(request, items).ConfigureAwait(false);

        return await invoiceService.AddInvoice(invoice).ConfigureAwait(false);
    }

    private async Task<Order[]> FetchOrdersAsync(IEnumerable<DateTime> dates)
    {
        var uniqueDates = dates.Select(d => d.Date).Distinct();
        var fetchTasks = uniqueDates.Select(distributorsSalesService.GetOrdersAsync);

        var ordersPerDate = await Task.WhenAll(fetchTasks).ConfigureAwait(false);
        return [.. ordersPerDate.SelectMany(x => x)];
    }

    private static Order[] FilterOrdersByIds(IEnumerable<Order> orders, IEnumerable<string> ids)
    {
        var idSet = new HashSet<string>(ids);
        return [.. orders.Where(o => idSet.Contains(o.Id))];
    }

    private List<InvoiceItem> MapInvoiceItems(Order[] orders, IDictionary<string, Product> products)
    {
        return [.. orders
            .SelectMany(order => order.Items)
            .Where(item => item != null && item.TotalPrice > 0)
            .Select(item =>
            {
                var partNumber = item.PartNumber ?? string.Empty;
                var productName = products.TryGetValue(partNumber, out var product)
                    ? $"{product.Name} ({partNumber})"
                    : partNumber;

                return new InvoiceItem
                {
                    Quantity = item.Quantity,
                    UnitPrice = Math.Round(item.TotalPrice, 2),
                    Unit = "sztuk",
                    Name = productName,
                    VatRate = 0.23M,
                    VatType = "PRC"
                };
            })];
    }

    private async Task<Invoice> CreateInvoiceAsync(CreateInvoiceCommand request, List<InvoiceItem> items)
    {
        var contractor = await context.Contractors.FindAsync(request.ContractorId)
            ?? throw new Exception($"Contractor with ID {request.ContractorId} not found");

        return new Invoice
        {
            Number = request.InvoiceNumber,
            IssueDate = request.IssueDate,
            SaleDate = request.SaleDate,
            PaymentDue = DateTime.Today.AddDays(14), // Default 14 days
            ContractorId = request.ContractorId,
            Contractor = contractor,
            Items = items
        };
    }
}
