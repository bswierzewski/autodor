using System.Text;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Options;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Invoices.Commands.CreateAllInvoices;

public class CreateAllInvoicesCommand : IRequest<Unit>
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class CreateAllInvoicesCommandHandler(
    IApplicationDbContext context,
    IInvoiceService invoiceService,
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
        var responses = new List<(string CustomerNumber, bool IsSuccess, string ErrorMessage)>();

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
                var invoiceItems = new List<InvoiceItem>();

                var items = groupedOrder.Value.SelectMany(x => x.Items);

                foreach (var item in items)
                {
                    if (item.TotalPrice <= 0)
                        continue;

                    var existsName = products.ContainsKey(item?.PartNumber ?? "");
                    var productName = existsName ? products[item.PartNumber].Name : item.PartNumber;
                    var fullName = existsName ? $"{productName} ({item.PartNumber})" : item.PartNumber;

                    invoiceItems.Add(new InvoiceItem
                    {
                        Name = fullName,
                        Unit = "sztuk",
                        Quantity = item.Quantity,
                        UnitPrice = Math.Round(item.TotalPrice * 1.23M, 2),
                        VatRate = 0.23M,
                        VatType = "PRC"
                    });
                }

                var contractor = contractors.FirstOrDefault(x => groupedOrder.Key.Contains(x.NIP))
                    ?? throw new Exception($"Podany CustomerNumber nie pasuje do żadnego kontrahenta. Zweryfikuj poprawność danych.");

                var invoice = CreateInvoice(contractor, invoiceItems);

                var result = await invoiceService.AddInvoice(invoice);

                responses.Add((result.IsSuccess ? result.Value : groupedOrder.Key, result.IsSuccess, result.Error));
            }
            catch (Exception ex)
            {
                responses.Add((groupedOrder.Key, false, ex.Message));

                logger.LogError(ex, ex.Message);
            }
        }

        await notificationService.Send(emailOptions.Value.To, $"Automat {polcarOptions.Value.DistributorCode} | {DateTime.Now}", FormatResponses(responses));

        return await Task.FromResult(Unit.Value);
    }

    private Invoice CreateInvoice(Contractor contractor, List<InvoiceItem> items)
    {
        return new Invoice
        {
            Number = null, // Auto-generated
            IssueDate = DateTime.Now,
            SaleDate = DateTime.Now,
            PaymentDue = DateTime.Now.AddDays(30),
            ContractorId = contractor.Id,
            Contractor = contractor,
            Items = items
        };
    }

    private string FormatResponses(List<(string CustomerNumber, bool IsSuccess, string ErrorMessage)> responses)
    {
        if (responses.Count == 0)
            return "<h3>Brak faktur do wystawienia w podanym zakresie dat.</h3>";

        var sb = new StringBuilder();
        sb.AppendLine("<html><head><style>");
        sb.AppendLine("body { font-family: Arial, sans-serif; }");
        sb.AppendLine("table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
        sb.AppendLine("th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }");
        sb.AppendLine("th { background-color: #f2f2f2; }");
        sb.AppendLine("h2 { color: #333333; }");
        sb.AppendLine(".success { color: green; }");
        sb.AppendLine(".error { color: red; }");
        sb.AppendLine("</style></head><body>");

        var correctResponses = responses.Where(x => x.IsSuccess).ToList();
        var errorResponses = responses.Where(x => !x.IsSuccess).ToList();

        if (correctResponses.Count > 0)
        {
            sb.AppendLine("<h2><span class='success'>✔</span> Poprawnie wystawione faktury</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Numer Klienta</th><th>Status</th></tr>");
            foreach (var response in correctResponses)
            {
                sb.AppendLine($"<tr><td>{response.CustomerNumber}</td><td>Sukces</td></tr>");
            }
            sb.AppendLine("</table>");
        }

        if (errorResponses.Count > 0)
        {
            sb.AppendLine("<h2><span class='error'>✖</span> Wykryte błędy</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Numer Klienta</th><th>Błąd</th></tr>");
            foreach (var response in errorResponses)
            {
                sb.AppendLine($"<tr><td>{response.CustomerNumber}</td><td>{response.ErrorMessage}</td></tr>");
            }
            sb.AppendLine("</table>");
        }

        sb.AppendLine("</body></html>");

        return sb.ToString();
    }

}
