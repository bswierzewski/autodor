using Autodor.Modules.Orders.Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.Commands.GenerateWarehouseDocument;

public class GenerateWarehouseDocumentCommandHandler(
    IOrdersRepository ordersRepository,
    IPdfDocumentService pdfDocumentService,
    ILogger<GenerateWarehouseDocumentCommandHandler> logger) : IRequestHandler<GenerateWarehouseDocumentCommand, ErrorOr<byte[]>>
{
    public async Task<ErrorOr<byte[]>> Handle(GenerateWarehouseDocumentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generating warehouse document for order {OrderId} on date {Date}",
            request.OrderId, request.Date);

        var order = await ordersRepository.GetOrderByIdAndDateAsync(request.OrderId, request.Date);

        if (order == null)
        {
            logger.LogWarning("Order {OrderId} not found for date {Date}", request.OrderId, request.Date);
            return Error.NotFound(
                code: "Order.NotFound",
                description: $"Order '{request.OrderId}' not found for date {request.Date:yyyy-MM-dd}");
        }

        var pdfBytes = await pdfDocumentService.GenerateWarehouseDocumentAsync(order, request.Date, cancellationToken);

        logger.LogInformation("Successfully generated warehouse document for order {OrderId}", request.OrderId);

        return pdfBytes;
    }
}