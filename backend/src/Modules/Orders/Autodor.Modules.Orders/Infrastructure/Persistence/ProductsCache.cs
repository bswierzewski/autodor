using Autodor.Modules.Orders.Abstractions.Services;
using Autodor.Modules.Orders.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

/// <summary>
/// In-memory cache implementation for products using IMemoryCache.
/// </summary>
public sealed class ProductsCache(
    IMemoryCache memoryCache,
    ILogger<ProductsCache> logger
) : IProductsCache
{
    private const string CacheKey = "products_dictionary";
    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        Priority = CacheItemPriority.NeverRemove,
        Size = 1
    };

    private FrozenDictionary<string, Product> Products =>
        memoryCache.Get<FrozenDictionary<string, Product>>(CacheKey) ?? FrozenDictionary<string, Product>.Empty;

    /// <summary>
    /// Gets a single product by number from cache.
    /// </summary>
    public Product? GetByNumber(string number) =>
        Products.GetValueOrDefault(number);

    /// <summary>
    /// Gets products by their numbers from cache.
    /// </summary>
    public Task<List<Product>> GetByNumbersAsync(string[] numbers)
        => Task.FromResult(numbers.Where(Products.ContainsKey).Select(n => Products[n]).ToList());

    /// <summary>
    /// Sets products in cache, replacing existing entries.
    /// </summary>
    public void Set(IEnumerable<Product> products)
    {
        var dict = products
            .GroupBy(p => p.Number)
            .ToFrozenDictionary(g => g.Key, g => g.Last());

        memoryCache.Set(CacheKey, dict, CacheOptions);

        logger.LogInformation("Products cache updated with {Count} products", dict.Count);
    }

    /// <summary>
    /// Gets the count of products in cache.
    /// </summary>
    public int Count => Products.Count;
}
