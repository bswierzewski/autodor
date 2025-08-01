using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Options;
using Application.Interfaces;
using Application.Invoices.Commands.DTOs;
using Application.Invoices.Extensions;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Application.Invoices.Commands.CreateAllInvoices;

public class CreateAllInvoicesCommand : IRequest<Unit>
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class CreateAllInvoicesCommandHandler(IMapper mapper,
    IApplicationDbContext context,
    IFirmaService firmaService,
    IDistributorsSalesService distributorsSalesService,
    IProductsService productsService,
    INotificationService notificationService,
    IOptions<EmailOptions> emailOptions,
    IOptions<PolcarOptions> polcarOptions,
    ILogger<CreateAllInvoicesCommandHandler> logger) : IRequestHandler<CreateAllInvoicesCommand, Unit>
{
    public async Task<Unit> Handle(CreateAllInvoicesCommand request, CancellationToken cancellationToken)
    {
        var orders = new List<Order>();
        var responses = new List<InvoiceResponseDto>();

        // Fetch products and contractors concurrently
        var productsTask = productsService.GetProductsAsync();
        var contractorsTask = context.Contractors.ToListAsync();

        // Fetch all orders within the date range concurrently
        var allDates = DateTimeExtensions.EachDay(request.DateFrom, request.DateTo);
        var allOrdersTasks = allDates.Select(distributorsSalesService.GetOrdersAsync);
        var ordersList = await Task.WhenAll(allOrdersTasks);
        orders = ordersList.SelectMany(o => o).ToList();

        // Get excluded orders as a HashSet to allow O(1) lookup
        var excludedOrders = context.ExcludedOrders
            .Select(x => x.OrderId)
            .ToHashSet();

        // Filter out excluded orders
        orders = orders.Where(x => !excludedOrders.Contains(x.Id)).ToList();

        // Group orders by CustomerNumber
        var groupedOrders = orders
            .GroupBy(x => x.CustomerNumber)
            .ToDictionary(g => g.Key, g => g.ToList());

        var products = await productsTask;
        var contractors = await contractorsTask;

        foreach (var groupedOrder in groupedOrders)
        {
            try
            {
                var pozycje = new List<Pozycje>();

                var items = groupedOrder.Value.SelectMany(x => x.Items);

                foreach (var item in items)
                {
                    if (item.TotalPrice <= 0)
                        continue;

                    var existsName = products.ContainsKey(item?.PartNumber ?? "");

                    pozycje.Add(new Pozycje
                    {
                        Ilosc = item.Quantity,
                        CenaJednostkowa = (float)Math.Round(item.TotalPrice * 1.23M, 2),
                        Jednostka = "sztuk",
                        NazwaPelna = $"{(existsName ? products[item.PartNumber].Name : item.PartNumber)}{(existsName ? $" ({item.PartNumber})" : "")}",
                        StawkaVat = 0.23M,
                        TypStawkiVat = "PRC",
                    });
                }

                var contractor = contractors.FirstOrDefault(x => groupedOrder.Key.Contains(x.NIP))
                    ?? throw new Exception($"Podany CustomerNumber nie pasuje do żadnego kontrahenta. Zweryfikuj poprawność danych.");

                var invoice = CreateInvoiceDto(contractor, pozycje);

                var response = await firmaService.AddInvoice(invoice);

                var responseText = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<InvoiceResponseDto>(responseText);
                result.CustomerNumber = groupedOrder.Key;

                responses.Add(result);
            }
            catch (Exception ex)
            {
                responses.Add(new InvoiceResponseDto
                {
                    CustomerNumber = groupedOrder.Key,
                    Response = new ResponseDto
                    {
                        Kod = 500,
                        Informacja = ex.Message
                    }
                });

                logger.LogError(ex, ex.Message);
            }
        }

        await notificationService.Send(emailOptions.Value.To, $"Automat {polcarOptions.Value.DistributorCode} | {DateTime.Now}", FormatResponses(responses));

        return await Task.FromResult(Unit.Value);
    }

    private InvoiceDto CreateInvoiceDto(Contractor contractor, List<Pozycje> pozycje)
    {
        var invoice = InvoiceExtensions.CreateDefaultInvoiceDto(pozycje);

        var kontrahent = mapper.Map<Kontrahent>(contractor);

        invoice.Numer = null;
        invoice.DataWystawienia = DateTime.Now.ToString("yyyy-MM-dd");
        invoice.DataSprzedazy = DateTime.Now.ToString("yyyy-MM-dd");
        invoice.Kontrahent = kontrahent;

        return invoice;
    }

    private string FormatResponses(List<InvoiceResponseDto> responses)
    {
        if (responses.Count == 0)
            return "Brak faktur do wystawienia";

        var sb = new StringBuilder();

        var correctResponses = responses.Where(x => x.Response.Kod == 0).ToList();
        var errorResponses = responses.Where(x => x.Response.Kod > 0).ToList();

        sb.AppendLine("<pre>");

        if (correctResponses.Count > 0)
        {
            sb.AppendLine("<strong>Poprawinie wystawiono faktury dla:</strong>");
            foreach (var response in correctResponses)
                sb.AppendLine($"    {response}");
        }

        if (errorResponses.Count > 0)
        {
            sb.AppendLine("<strong>Wykryto błąd dla:</strong>");
            foreach (var response in errorResponses)
                sb.AppendLine($"    {response}");
        }

        sb.AppendLine("</pre>");

        return sb.ToString();
    }

}
