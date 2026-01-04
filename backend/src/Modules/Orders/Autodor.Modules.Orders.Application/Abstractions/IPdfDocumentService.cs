using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Application.Abstractions;

public interface IPdfDocumentService
{
    Task<byte[]> GenerateWarehouseDocumentAsync(Order order, DateTime documentDate, CancellationToken cancellationToken = default);
}