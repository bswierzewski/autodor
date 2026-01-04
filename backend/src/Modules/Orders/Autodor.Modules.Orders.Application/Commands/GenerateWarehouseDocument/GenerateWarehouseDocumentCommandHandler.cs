using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.Commands.GenerateWarehouseDocument;

public class GenerateWarehouseDocumentCommandHandler(
    IOrdersRepository ordersRepository,
    IPdfDocumentService pdfDocumentService,
    ILogger<GenerateWarehouseDocumentCommandHandler> logger) : IRequestHandler<GenerateWarehouseDocumentCommand, byte[]>
{
    public async Task<byte[]> Handle(GenerateWarehouseDocumentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating warehouse document for order {OrderId} on date {Date}",
            request.OrderId, request.Date);

        var order = await ordersRepository.GetOrderByIdAndDateAsync(request.OrderId, request.Date);

        if (order == null)
        {
            logger.LogWarning("Order {OrderId} not found for date {Date}", request.OrderId, request.Date);
            throw new KeyNotFoundException($"Order {request.OrderId} not found for date {request.Date:yyyy-MM-dd}");
        }

        var pdfBytes = await pdfDocumentService.GenerateWarehouseDocumentAsync(order, request.Date, cancellationToken);

        logger.LogInformation("Successfully generated warehouse document for order {OrderId}", request.OrderId);

        return pdfBytes;
    }
}