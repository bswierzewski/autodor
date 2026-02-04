using Autodor.Modules.Orders.Domain.Models;

namespace Autodor.Modules.Orders.Abstractions.Integrations;

/// <summary>
/// Service for products external integration
/// </summary>
public interface IProductsService
{
    /// <summary>
    /// Gets all products from the external system.
    /// </summary>
    /// <returns>A collection of products.</returns>
    Task<IEnumerable<Product>> GetProductsAsync();
}
