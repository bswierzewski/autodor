using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.API;

/// <summary>
/// Implements the public API for the Products module, providing access to product information.
/// </summary>
public sealed class ProductsAPI : IProductsAPI
{
    private readonly IProductsDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the ProductsAPI class.
    /// </summary>
    /// <param name="dbContext">The database context for products</param>
    public ProductsAPI(IProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a single product by its number.
    /// </summary>
    /// <param name="number">The product number to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The product details if found and has a name, otherwise null</returns>
    public async Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);

        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        return product.ToDto();
    }

    /// <summary>
    /// Retrieves multiple products by their numbers in a single query.
    /// </summary>
    /// <param name="numbers">The collection of product numbers to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Collection of product details for found products</returns>
    public async Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Products
            .AsNoTracking()
            .Where(x => numbers.Contains(x.Number))
            .ToListAsync(cancellationToken);

        return products.Select(p => p.ToDto());
    }
}