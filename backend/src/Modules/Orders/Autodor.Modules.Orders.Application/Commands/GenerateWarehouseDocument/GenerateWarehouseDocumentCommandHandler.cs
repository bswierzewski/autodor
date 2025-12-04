using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Orders.Application.Commands.GenerateWarehouseDocument;

/// <summary>
/// Handler for generating warehouse document (Wydanie Zewnętrzne) PDF.
/// This handler processes the warehouse document generation request by collecting
/// order data and generating a formatted PDF document for warehouse operations.
/// </summary>
public class GenerateWarehouseDocumentCommandHandler : IRequestHandler<GenerateWarehouseDocumentCommand, Result<byte[]>>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IPdfDocumentService _pdfDocumentService;
    private readonly ILogger<GenerateWarehouseDocumentCommandHandler> _logger;

    public GenerateWarehouseDocumentCommandHandler(
        IOrdersRepository ordersRepository,
        IPdfDocumentService pdfDocumentService,
        ILogger<GenerateWarehouseDocumentCommandHandler> logger)
    {
        _ordersRepository = ordersRepository;
        _pdfDocumentService = pdfDocumentService;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(GenerateWarehouseDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating warehouse document for order {OrderId} on date {Date}",
            request.OrderId, request.Date);

        var order = await _ordersRepository.GetOrderByIdAndDateAsync(request.OrderId, request.Date);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for date {Date}", request.OrderId, request.Date);
            return Result<byte[]>.Failure("ORDER_NOT_FOUND", $"Order {request.OrderId} not found for date {request.Date:yyyy-MM-dd}");
        }

        var pdfBytes = await _pdfDocumentService.GenerateWarehouseDocumentAsync(order, request.Date, cancellationToken);

        _logger.LogInformation("Successfully generated warehouse document for order {OrderId}", request.OrderId);

        return Result<byte[]>.Success(pdfBytes);
    }
}