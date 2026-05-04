using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;
using Wolverine;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

public static class GenerateDeliveryNoteHandler
{
    [WolverinePost("/delivery-notes")]
    [Tags("Orders")]
    [EndpointName("GenerateDeliveryNote")]
    [EndpointSummary("Generate PDF delivery note for an order")]
    public static async Task<IResult> Handle(
        GenerateDeliveryNoteCommand command,
        IOrderService orderService,
        IMessageBus bus,
        ILogger<GenerateDeliveryNoteCommand> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Generating delivery note for order {OrderId} on date {Date}", command.OrderId, command.Date);

        // Fetch order with exclusions marked (OrderService handles enrichment and marking)
        var order = await orderService.GetOrderAsync(command.OrderId, command.Date, ct);

        if (order is null)
            return Error.NotFound("Order.NotFound", $"Order with ID '{command.OrderId}' was not found").Problem();

        // Business logic: Filter out excluded order
        if (order.IsExcluded)
            return Error.NotFound("Order.Excluded", $"Order with ID '{command.OrderId}' is excluded").Problem();

        // Business logic: Filter out excluded items
        var nonExcludedItems = order.Items.Where(i => !i.IsExcluded).ToList();

        if (nonExcludedItems.Count == 0)
            return Error.NotFound("Order.NoItems", "Order has no items after exclusions are applied").Problem();

        // Fetch contractor by NIP (CustomerNumber)
        if (string.IsNullOrWhiteSpace(order.CustomerNumber))
            return Error.NotFound("Order.EmptyCustomerNumber", "Customer number is empty").Problem();

        var contractor = await bus.InvokeAsync<ContractorDto?>(new GetContractorByNipQuery(order.CustomerNumber), ct);

        if (contractor is null)
            return Error.NotFound("Contractor.NotFound", $"Contractor with NIP '{order.CustomerNumber}' was not found").Problem();

        // Generate PDF with non-excluded items
        var pdfBytes = CreateDocument(order, nonExcludedItems, contractor).GeneratePdf();

        // Return PDF file
        var fileName = $"WZ_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        return Results.File(pdfBytes, "application/pdf", fileName);
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
                var totalPrice = item.Price * item.Quantity;
                var vatAmount = Math.Round(totalPrice * 0.23M, 2);
                var grossAmount = Math.Round(totalPrice * 1.23M, 2);

                table.Cell().Element(CellStyle).Text(item.ProductDisplayName);
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice:F2} zł");
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
