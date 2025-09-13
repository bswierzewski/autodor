using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Provides functionality for generating PDF documents related to orders.
/// </summary>
public interface IPdfDocumentService
{
    /// <summary>
    /// Generates a warehouse document (Wydanie Zewnętrzne) PDF for the specified order.
    /// </summary>
    /// <param name="order">The order for which to generate the warehouse document.</param>
    /// <param name="documentDate">The date of the warehouse operation/document generation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A byte array containing the generated PDF document.</returns>
    Task<byte[]> GenerateWarehouseDocumentAsync(Order order, DateTime documentDate, CancellationToken cancellationToken = default);
}