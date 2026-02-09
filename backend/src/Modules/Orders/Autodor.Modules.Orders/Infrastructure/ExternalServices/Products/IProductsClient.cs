using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Dtos;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public interface IProductsClient
{
    /// <summary>
    /// Gets all products from the external system.
    /// </summary>
    /// <returns>A collection of products.</returns>
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
