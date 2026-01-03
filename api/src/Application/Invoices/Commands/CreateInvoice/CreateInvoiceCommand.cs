using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using System.Linq;

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
        // Start all tasks in parallel
        var ordersTask = FetchOrdersAsync(request.Dates);
        var productsTask = productsService.GetProductsAsync();
        var excludedPositionsTask = LoadExcludedPositionsAsync(request.OrderIds, cancellationToken);

        await Task.WhenAll(ordersTask, productsTask, excludedPositionsTask).ConfigureAwait(false);

        var allOrders = await ordersTask.ConfigureAwait(false);
        var products = await productsTask.ConfigureAwait(false);
        var excludedPositions = await excludedPositionsTask.ConfigureAwait(false);

        var filteredOrders = FilterOrdersByIds(allOrders, request.OrderIds);
        var items = MapInvoiceItems(filteredOrders, products, excludedPositions);
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

    private async Task<Dictionary<string, HashSet<string>>> LoadExcludedPositionsAsync(
        IEnumerable<string> orderIds,
        CancellationToken cancellationToken)
    {
        return await context.ExcludedOrderPositions
            .Where(x => orderIds.Contains(x.OrderId))
            .GroupBy(x => x.OrderId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(x => x.PartNumber).ToHashSet()
            , cancellationToken);
    }

    private List<InvoiceItem> MapInvoiceItems(Order[] orders, IDictionary<string, Product> products, Dictionary<string, HashSet<string>> excludedPositions)
    {
        return [.. orders
            .SelectMany(order => order.Items.Select(item => new { Order = order, Item = item }))
            .Where(x => x.Item != null && x.Item.TotalPrice > 0)
            .Where(x => !excludedPositions.TryGetValue(x.Order.Id, out var excluded) || !excluded.Contains(x.Item.PartNumber))
            .Select(x =>
            {
                var partNumber = x.Item.PartNumber ?? string.Empty;
                var productName = products.TryGetValue(partNumber, out var product)
                    ? $"{product.Name} ({partNumber})"
                    : partNumber;

                return new InvoiceItem
                {
                    Quantity = x.Item.Quantity,
                    UnitPrice = Math.Round(x.Item.TotalPrice, 2),
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
