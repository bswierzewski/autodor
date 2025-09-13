using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Shared.Contracts.Products;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Autodor.Modules.Orders.Infrastructure.Services;

public class PdfDocumentService : IPdfDocumentService
{
    private readonly IProductsAPI _productsAPI;
    private readonly ILogger<PdfDocumentService> _logger;

    public PdfDocumentService(IProductsAPI productsAPI, ILogger<PdfDocumentService> logger)
    {
        _productsAPI = productsAPI;
        _logger = logger;
    }

    public async Task<byte[]> GenerateWarehouseDocumentAsync(Order order, DateTime documentDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating warehouse document for order {OrderId} on date {Date}", order.Id, documentDate);

        var productNumbers = order.Items.Select(i => i.Number).ToList();
        var products = await _productsAPI.GetProductsByNumbersAsync(productNumbers, cancellationToken);
        var productsDictionary = products.ToDictionary(p => p.Number, p => p);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);

                page.Content().Column(column =>
                {
                    column.Item().Element(ComposeHeader);
                    column.Item().PaddingVertical(10).Element(container => ComposeClientInfo(container, order.Contractor, documentDate, order.Number));
                    column.Item().PaddingVertical(10).Element(container => ComposeItemsTable(container, order.Items, productsDictionary));
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Wydanie Zewnętrzne")
                    .FontSize(24)
                    .Bold()
                    .AlignCenter();
            });

            row.ConstantItem(100).AlignRight().Image(GetLogoPath());
        });
    }

    private void ComposeClientInfo(IContainer container, OrderContractor contractor, DateTime documentDate, string orderNumber)
    {
        container.Column(column =>
        {
            column.Item().Text($"Data: {documentDate:dd-MM-yyyy}").FontSize(12);
            column.Item().Text($"Numer Zlecenia: {orderNumber}").FontSize(12);
            column.Item().Text($"Klient: {contractor.Name}").FontSize(12);
        });
    }

    private void ComposeItemsTable(IContainer container, ICollection<OrderItem> items, Dictionary<string, Autodor.Shared.Contracts.Products.Dtos.ProductDetailsDto> productsDictionary)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3); // Nazwa
                columns.RelativeColumn(1); // Ilość
                columns.RelativeColumn(2); // Wartość netto
                columns.RelativeColumn(2); // Wartość VAT
                columns.RelativeColumn(2); // Wartość Brutto
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Nazwa").Bold();
                header.Cell().Element(CellStyle).Text("Ilość").Bold();
                header.Cell().Element(CellStyle).Text("Wartość netto").Bold();
                header.Cell().Element(CellStyle).Text("Wartość VAT").Bold();
                header.Cell().Element(CellStyle).Text("Wartość Brutto").Bold();
            });

            foreach (var item in items)
            {
                var productName = productsDictionary.TryGetValue(item.Number, out var product) 
                    ? product.Name 
                    : $"Produkt {item.Number}";

                table.Cell().Element(CellStyle).Text($"{productName} ({item.Number})");
                table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).Text($"{item.NetValue:F2} zł");
                table.Cell().Element(CellStyle).Text($"{item.VatValue:F2} zł");
                table.Cell().Element(CellStyle).Text($"{item.GrossValue:F2} zł");
            }
        });
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .AlignCenter()
            .AlignMiddle();
    }

    private string GetLogoPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        return Path.Combine(currentDirectory, "Modules", "Orders", "Autodor.Modules.Orders.Infrastructure", "Resources", "Images", "Logo.png");
    }
}