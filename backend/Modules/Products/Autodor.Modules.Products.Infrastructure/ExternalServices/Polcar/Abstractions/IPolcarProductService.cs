using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;

/// <summary>
/// Service interface for retrieving product data from the Polcar external system.
/// Abstracts the complexity of SOAP communication and provides a clean domain interface.
/// </summary>
public interface IPolcarProductService
{
    /// <summary>
    /// Retrieves all available products from the Polcar external service.
    /// This operation fetches the complete product catalog with part numbers, names, and EAN codes.
    /// </summary>
    /// <returns>Collection of Product entities representing the external catalog, or empty collection on failure</returns>
    Task<IEnumerable<Product>> GetProductsAsync();
}