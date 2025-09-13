using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.GenerateWarehouseDocument;

/// <summary>
/// Command for generating a warehouse document (Wydanie Zewnętrzne) as PDF.
/// This command creates a warehouse external release document based on a specific order
/// and date, which serves as proof of goods being released from the warehouse.
/// The document is generated as a PDF for printing and archival purposes.
/// </summary>
/// <param name="OrderId">Identifier of the order for which to generate the warehouse document</param>
/// <param name="Date">Date of the warehouse operation/document generation</param>
public record GenerateWarehouseDocumentCommand(
    string OrderId,
    DateTime Date
) : IRequest<byte[]>; // Returns PDF as byte array