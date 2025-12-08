using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of products repository using ConcurrentDictionary for thread-safe operations.
/// Optimized for read-heavy workloads with periodic bulk updates.
/// </summary>
/// <remarks>
/// Initializes a new instance of the InMemoryProductsRepository.
/// </remarks>
/// <param name="logger">Logger for repository operations</param>
public class InMemoryProductsRepository(ILogger<InMemoryProductsRepository> logger) : IProductsRepository
{
    private ConcurrentDictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Product? GetProductByNumber(string number)
    {
        _products.TryGetValue(number, out var product);
        return product;
    }

    /// <inheritdoc />
    public IEnumerable<Product> GetProductsByNumbers(IEnumerable<string> numbers)
    {
        var result = new List<Product>();
        foreach (var number in numbers)
        {
            if (_products.TryGetValue(number, out var product))
            {
                result.Add(product);
            }
        }
        return result;
    }

    /// <inheritdoc />
    public void ReplaceAllProducts(IEnumerable<Product> products)
    {
        var productsList = products.ToList();
        var duplicatesCount = 0;

        // Create dictionary with duplicate handling - last occurrence wins
        var dictionary = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
        foreach (var product in productsList)
        {
            if (dictionary.ContainsKey(product.Number))
            {
                duplicatesCount++;
                logger.LogWarning(
                    "Duplicate product number detected: {ProductNumber}. Overwriting with latest occurrence.",
                    product.Number);
            }

            dictionary[product.Number] = product; // Overwrites if exists
        }

        var newDictionary = new ConcurrentDictionary<string, Product>(dictionary);
        var oldDictionary = Interlocked.Exchange(ref _products, newDictionary);

        logger.LogInformation(
            "Products repository updated: {NewCount} products loaded, replaced {OldCount} products. {Duplicates} duplicates handled.",
            newDictionary.Count,
            oldDictionary.Count,
            duplicatesCount);
    }

    /// <inheritdoc />
    public int GetProductCount()
    {
        return _products.Count;
    }
}
