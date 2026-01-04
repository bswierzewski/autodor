using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

public class InMemoryProductsRepository(ILogger<InMemoryProductsRepository> logger) : IProductsRepository
{
    private ConcurrentDictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase);

    public Product? GetProductByNumber(string number)
    {
        _products.TryGetValue(number, out var product);
        return product;
    }

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

    public int GetProductCount() => _products.Count;
}
