using System.Reflection;
using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Wolverine;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public static class GenerateDeliveryNoteHandler
{
    [Authorize]
    public static async Task<FileContentHttpResult> Handle(
        GenerateDeliveryNoteCommand command,
        IOrderService orderService,
        IMessageBus bus,
        ILogger<GenerateDeliveryNoteCommand> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Generating delivery note for order {OrderId} on date {Date}", command.OrderId, command.Date);

        // Fetch order with exclusions marked (OrderService handles enrichment and marking)
        var order = await orderService.GetOrderAsync(command.OrderId, command.Date, ct) ?? throw new NotFoundException($"Nie znaleziono zamówienia o identyfikatorze '{command.OrderId}'.");

        // Business logic: Filter out excluded order
        if (order.IsExcluded)
            throw new NotFoundException($"Zamówienie o identyfikatorze '{command.OrderId}' jest wykluczone.");

        // Business logic: Filter out excluded items
        var nonExcludedItems = order.Items.Where(i => !i.IsExcluded).ToList();

        if (nonExcludedItems.Count == 0)
            throw new NotFoundException("Po zastosowaniu wykluczeń zamówienie nie zawiera żadnych pozycji.");

        // Fetch contractor by NIP (CustomerNumber)
        if (string.IsNullOrWhiteSpace(order.CustomerNumber))
            throw new NotFoundException("Numer klienta jest pusty.");

        var contractor = await bus.InvokeAsync<ContractorDto?>(new GetContractorByNIPQuery(order.CustomerNumber), ct) ?? throw new NotFoundException($"Nie znaleziono kontrahenta o numerze NIP '{order.CustomerNumber}'.");

        // Generate PDF with non-excluded items
        var pdfBytes = CreateDocument(order, nonExcludedItems, contractor).GeneratePdf();

        // Return PDF file
        var fileName = $"WZ_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        return TypedResults.File(pdfBytes, "application/pdf", fileName);
    }

    private static Document CreateDocument(Order order, List<OrderItem> items, ContractorDto contractor)
    {
        // Generate PDF using QuestPDF
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial));

                page.Content().Column(column =>
                {
                    column.Item().Element(c => ComposeHeader(c, order, contractor));
                    column.Item().PaddingVertical(15);
                    column.Item().Element(c => ComposeTable(c, items));
                });
            });
        });
    }

    private static void ComposeHeader(IContainer container, Order order, ContractorDto contractor)
    {
        container.Column(column =>
        {
            // Title
            column.Item()
                .PaddingBottom(10)
                .AlignCenter()
                .Text("Wydanie Zewnętrzne")
                .FontSize(24)
                .Bold();

            column.Item().Row(row =>
            {
                // Left side - Order and contractor info
                row.RelativeItem().Column(col =>
                {
                    void AddAttribute(string label, string value)
                    {
                        col.Item().Text(text =>
                        {
                            text.Span(label).Bold();
                            text.Span(value);
                        });
                    }

                    AddAttribute("Data: ", order.Date.ToString("dd-MM-yyyy"));
                    AddAttribute("Numer Zlecenia: ", order.Number ?? "");
                    AddAttribute("Klient: ", contractor.Name);
                    AddAttribute("Adres: ", $"{contractor.Street}, {contractor.ZipCode} {contractor.City}");
                    AddAttribute("NIP: ", contractor.NIP);
                    AddAttribute("Email: ", contractor.Email ?? "");
                });

                // Right side - Logo
                row.ConstantItem(150)
                    .AlignMiddle()
                    .Image(LoadEmbeddedResource("Resources.Images.Logo.png"));
            });
        });
    }

    private static void ComposeTable(IContainer container, List<OrderItem> items)
    {
        container.Table(table =>
        {
            // Define columns
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(5); // Nazwa
                columns.RelativeColumn(0.8f); // Ilość
                columns.RelativeColumn(1.5f); // Wartość netto
                columns.RelativeColumn(1.5f); // Wartość VAT
                columns.RelativeColumn(1.5f); // Wartość Brutto
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(HeaderCellStyle).Text("Nazwa").Bold();
                header.Cell().Element(HeaderCellStyle).Text("Ilość").AlignRight().Bold();
                header.Cell().Element(HeaderCellStyle).Text("Wartość\nNetto").AlignRight().Bold();
                header.Cell().Element(HeaderCellStyle).Text("Wartość\nVAT").AlignRight().Bold();
                header.Cell().Element(HeaderCellStyle).Text("Wartość\nBrutto").AlignRight().Bold();

                static IContainer HeaderCellStyle(IContainer container)
                {
                    return container
                        .Border(0.5f)
                        .BorderColor(Colors.Black)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(5);
                }
            });

            // Items
            foreach (var item in items)
            {
                var netAmount = item.Price;
                var vatAmount = Math.Round(netAmount * 0.23M, 2);
                var grossAmount = Math.Round(netAmount + vatAmount, 2);

                table.Cell().Element(CellStyle).Text(item.ProductDisplayName);
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text($"{netAmount:F2} zł");
                table.Cell().Element(CellStyle).AlignRight().Text($"{vatAmount:F2} zł");
                table.Cell().Element(CellStyle).AlignRight().Text($"{grossAmount:F2} zł");

                static IContainer CellStyle(IContainer container)
                {
                    return container
                        .Border(0.5f)
                        .BorderColor(Colors.Black)
                        .Padding(5);
                }
            }
        });
    }

    private static byte[] LoadEmbeddedResource(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream($"Autodor.Modules.Orders.{resourcePath}");

        if (stream is null)
            return [];

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
