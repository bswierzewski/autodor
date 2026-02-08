using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Dtos;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public interface IProductsService
{
    /// <summary>
    /// Gets all products from the external system.
    /// </summary>
    /// <returns>A collection of products.</returns>
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
