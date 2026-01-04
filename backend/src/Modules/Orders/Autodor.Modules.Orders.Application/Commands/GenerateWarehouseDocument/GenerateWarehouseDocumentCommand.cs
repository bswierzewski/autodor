using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.GenerateWarehouseDocument;

/// <summary>
/// Command for generating a warehouse document (Wydanie Zewnętrzne) as PDF.
/// </summary>
public record GenerateWarehouseDocumentCommand(
    string OrderId,
    DateTime Date
) : IRequest<byte[]>;