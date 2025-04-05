using Application.Common.Consts;
using Application.Common.Interfaces;
using Application.Interfaces;
using Application.Invoices.Commands.DTOs;
using Domain.Entities;

namespace Application.Invoices.Commands.PrintInvoice;

public class PrintInvoiceCommand : IRequest<FileInvoiceResponseDto>
{
    public string OrderId { get; set; }
    public DateTime Date { get; set; }
}

public class PrintInvoiceCommandHandler(
    IPDFGeneratorService pdfGeneratorService,
    IDistributorsSalesService distributorsSalesService,
    IApplicationDbContext context,
    IProductsService productsService,
    ICacheService cacheService,
    IHtmlGeneratorService htmlTemplateGenerator) : IRequestHandler<PrintInvoiceCommand, FileInvoiceResponseDto>
{
    public async Task<FileInvoiceResponseDto> Handle(PrintInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(request);
        var products = await GetProductsAsync();
        var contractor = await GetContractorAsync(order.CustomerNumber) 
            ?? throw new Exception($"Contractor for {order.CustomerNumber} not found");

        // Enrich order with product names
        EnrichOrderWithProductNames(order, products);

        var htmlContent = htmlTemplateGenerator.Generate(contractor, order);
        var content = pdfGeneratorService.Generate(htmlContent);

        return CreateFileResponse(order, content);
    }

    private async Task<Order> GetOrderAsync(PrintInvoiceCommand request)
    {
        var orders = await distributorsSalesService.GetOrdersAsync(request.Date);
        return orders.FirstOrDefault(x => x.Id == request.OrderId)
            ?? throw new Exception($"Order not found: {request.OrderId}");
    }

    private async Task<IDictionary<string, Product>> GetProductsAsync()
    {
        return await cacheService.GetOrCreateAsync(
            CacheConsts.Products,
            productsService.GetProductsAsync,
            TimeSpan.FromHours(1));
    }

    private async Task<Contractor> GetContractorAsync(string customerNumber)
    {
        var contractors = await context.Contractors.ToListAsync();
        return contractors.FirstOrDefault(x => customerNumber.Contains(x.NIP));
    }

    private void EnrichOrderWithProductNames(Order order, IDictionary<string, Product> products)
    {
        foreach (var item in order.Items)
        {
            var existsName = products.ContainsKey(item?.PartNumber ?? "");
            item.PartName = existsName
                ? $"{products[item.PartNumber].Name} ({item.PartNumber})"
                : item.PartNumber;
        }
    }

    private FileInvoiceResponseDto CreateFileResponse(Order order, byte[] content)
    {
        return new FileInvoiceResponseDto
        {
            FileName = $"{order.Number}.pdf",
            ContentType = "application/pdf",
            Content = content
        };
    }
}

