using System.Collections.Frozen;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public interface IProductsClient
{
    /// <summary>
    /// Gets all products from the external system as a frozen dictionary keyed by product number.
    /// </summary>
    /// <returns>A frozen dictionary of products keyed by product number.</returns>
    Task<FrozenDictionary<string, Product>> GetProductsAsync();
}
