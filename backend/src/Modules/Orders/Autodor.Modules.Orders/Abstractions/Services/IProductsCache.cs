using Autodor.Modules.Orders.Domain.Models;

namespace Autodor.Modules.Orders.Abstractions.Services;

/// <summary>
/// Abstraction for products cache operations.
/// </summary>
public interface IProductsCache
{
    /// <summary>
    /// Gets a single product by number from cache.
    /// </summary>
    Product? GetByNumber(string number);

    /// <summary>
    /// Gets products by their numbers from cache.
    /// </summary>
    Task<List<Product>> GetByNumbersAsync(string[] numbers);

    /// <summary>
    /// Sets products in cache, replacing existing entries.
    /// </summary>
    void Set(IEnumerable<Product> products);

    /// <summary>
    /// Gets the count of products in cache.
    /// </summary>
    int Count { get; }
}
