using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Modules.Products.Application.API;

/// <summary>
/// Implements the public API for the Products module, providing access to product information.
/// </summary>
public sealed class ProductsAPI : IProductsAPI
{
    private readonly IProductsRepository _repository;

    /// <summary>
    /// Initializes a new instance of the ProductsAPI class.
    /// </summary>
    /// <param name="repository">The in-memory repository for products</param>
    public ProductsAPI(IProductsRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves a single product by its number.
    /// </summary>
    /// <param name="number">The product number to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The product details if found and has a name, otherwise null</returns>
    public Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var product = _repository.GetProductByNumber(number);

        if (product == null || string.IsNullOrEmpty(product.Name))
            return Task.FromResult<ProductDetailsDto?>(null);

        return Task.FromResult<ProductDetailsDto?>(product.ToDto());
    }

    /// <summary>
    /// Retrieves multiple products by their numbers in a single query.
    /// </summary>
    /// <param name="numbers">The collection of product numbers to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Collection of product details for found products</returns>
    public Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        var products = _repository.GetProductsByNumbers(numbers);
        return Task.FromResult(products.Select(p => p.ToDto()));
    }
}