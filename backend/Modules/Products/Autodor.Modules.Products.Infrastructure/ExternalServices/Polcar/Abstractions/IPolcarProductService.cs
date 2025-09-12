using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;

/// <summary>
/// Defines the contract for retrieving products from the Polcar external system.
/// </summary>
public interface IPolcarProductService
{
    /// <summary>
    /// Retrieves all products from the Polcar external service.
    /// </summary>
    /// <returns>A collection of products from the external system</returns>
    Task<IEnumerable<Product>> GetProductsAsync();
}