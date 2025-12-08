using Autodor.Modules.Products.Domain.Entities;

namespace Autodor.Modules.Products.Application.Abstractions;

/// <summary>
/// Provides access to product data stored in memory.
/// </summary>
public interface IProductsRepository
{
    /// <summary>
    /// Gets a product by its number.
    /// </summary>
    /// <param name="number">The product number to search for</param>
    /// <returns>The product if found, otherwise null</returns>
    Product? GetProductByNumber(string number);

    /// <summary>
    /// Gets multiple products by their numbers.
    /// </summary>
    /// <param name="numbers">The collection of product numbers to search for</param>
    /// <returns>Collection of found products</returns>
    IEnumerable<Product> GetProductsByNumbers(IEnumerable<string> numbers);

    /// <summary>
    /// Replaces all products in the repository with the new collection.
    /// This is a thread-safe operation.
    /// </summary>
    /// <param name="products">The new collection of products</param>
    void ReplaceAllProducts(IEnumerable<Product> products);

    /// <summary>
    /// Gets the total count of products in the repository.
    /// </summary>
    /// <returns>The number of products</returns>
    int GetProductCount();
}
