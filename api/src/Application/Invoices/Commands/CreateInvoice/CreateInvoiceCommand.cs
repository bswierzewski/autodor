using System.Text.Json;
using Application.Common.Interfaces;
using Application.Invoices.Commands.DTOs;
using Application.Invoices.Extensions;
using AutoMapper;
using Domain.Entities;

namespace Application.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommand : IRequest<InvoiceResponseDto>
{
    public int? InvoiceNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public DateTime IssueDate { get; set; }
    public IEnumerable<DateTime> Dates { get; set; }
    public IEnumerable<string> OrderIds { get; set; }
    public int ContractorId { get; set; }
}

public class CreateInvoiceCommandHandler(IMapper mapper,
    IFirmaService firmaService,
    IDistributorsSalesService distributorsSalesService,
    IProductsService productsService,
    IApplicationDbContext context) : IRequestHandler<CreateInvoiceCommand, InvoiceResponseDto>
{
    public async Task<InvoiceResponseDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Start both tasks in parallel
        var ordersTask = FetchOrdersAsync(request.Dates);
        var productsTask = productsService.GetProductsAsync();

        await Task.WhenAll(ordersTask, productsTask).ConfigureAwait(false);

        var allOrders = await ordersTask.ConfigureAwait(false);
        var products = await productsTask.ConfigureAwait(false);

        var filteredOrders = FilterOrdersByIds(allOrders, request.OrderIds);
        var items = MapInvoiceItems(filteredOrders, products);
        var invoice = await CreateInvoiceDtoAsync(request, items).ConfigureAwait(false);

        var response = await firmaService.AddInvoice(invoice).ConfigureAwait(false);
        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<InvoiceResponseDto>(responseText);
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

    private List<Pozycje> MapInvoiceItems(Order[] orders, IDictionary<string, Product> products)
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

                return new Pozycje
                {
                    Ilosc = item.Quantity,
                    CenaJednostkowa = (float)Math.Round(item.TotalPrice * 1.23M, 2),
                    Jednostka = "sztuk",
                    NazwaPelna = productName,
                    StawkaVat = 0.23M,
                    TypStawkiVat = "PRC"
                };
            })];
    }

    private async Task<InvoiceDto> CreateInvoiceDtoAsync(CreateInvoiceCommand request, List<Pozycje> pozycje)
    {
        var contractor = await context.Contractors.FindAsync(request.ContractorId)
            ?? throw new Exception($"Contractor with ID {request.ContractorId} not found");

        var invoice = InvoiceExtensions.CreateDefaultInvoiceDto(pozycje);
        invoice.Numer = request.InvoiceNumber;
        invoice.DataWystawienia = request.IssueDate.ToString("yyyy-MM-dd");
        invoice.DataSprzedazy = request.SaleDate.ToString("yyyy-MM-dd");
        invoice.Kontrahent = mapper.Map<Kontrahent>(contractor);

        return invoice;
    }
}
