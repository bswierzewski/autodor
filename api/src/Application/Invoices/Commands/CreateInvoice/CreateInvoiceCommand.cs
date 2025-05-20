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
        var tasks = dates.Distinct().Select(distributorsSalesService.GetOrdersAsync);
        var ordersPerDate = await Task.WhenAll(tasks).ConfigureAwait(false);
        return ordersPerDate.SelectMany(x => x).ToArray();
    }

    private static Order[] FilterOrdersByIds(IEnumerable<Order> orders, IEnumerable<string> ids)
        => orders.Where(o => ids.Distinct().Contains(o.Id)).ToArray();

    private List<Pozycje> MapInvoiceItems(Order[] orders, IDictionary<string, Product> products)
    {
        return orders
            .SelectMany(order => order.Items)
            .Where(item => item.TotalPrice > 0)
            .Select(item =>
            {
                var existsName = products.ContainsKey(item?.PartNumber ?? "");
                var name = existsName ? $"{products[item.PartNumber].Name} ({item.PartNumber})" : item.PartNumber;

                return new Pozycje
                {
                    Ilosc = item.Quantity,
                    CenaJednostkowa = (float)Math.Round(item.TotalPrice * 1.23M, 2),
                    Jednostka = "sztuk",
                    NazwaPelna = name,
                    StawkaVat = 0.23M,
                    TypStawkiVat = "PRC"
                };
            })
            .ToList();
    }

    private async Task<InvoiceDto> CreateInvoiceDtoAsync(CreateInvoiceCommand request, List<Pozycje> pozycje)
    {
        var contractor = await context.Contractors.FindAsync(request.ContractorId)
            ?? throw new Exception("Contractor not found");

        var invoice = InvoiceExtensions.CreateDefaultInvoiceDto(pozycje);
        invoice.Numer = request.InvoiceNumber;
        invoice.DataWystawienia = request.IssueDate.ToString("yyyy-MM-dd");
        invoice.DataSprzedazy = request.SaleDate.ToString("yyyy-MM-dd");
        invoice.Kontrahent = mapper.Map<Kontrahent>(contractor);

        return invoice;
    }
}
