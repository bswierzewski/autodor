using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.API;

/// <summary>
/// Internal implementation of the Products API that provides read-only access to product data.
/// This API serves as the primary interface for other modules to query product information.
/// </summary>
internal sealed class ProductsAPI : IProductsAPI
{
    private readonly IProductsReadDbContext _readDbContext;

    /// <summary>
    /// Initializes a new instance of the ProductsAPI class.
    /// </summary>
    /// <param name="readDbContext">The read-only database context for product queries</param>
    public ProductsAPI(IProductsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Retrieves a single product by its part number.
    /// </summary>
    /// <param name="number">The product part number to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Product details if found and has a valid name, otherwise null</returns>
    public async Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        // Query the database for the product by part number
        var product = await _readDbContext.Products
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);

        // Return null if product not found or has invalid data (business rule: products must have names)
        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        // Map the domain entity to a contract DTO for external consumption
        return new ProductDetailsDto(
            Number: product.Number,
            Name: product.Name,
            EAN13: product.EAN13
        );
    }

    /// <summary>
    /// Retrieves multiple products by their part numbers in a single query.
    /// This method is optimized for bulk operations and reduces database round trips.
    /// </summary>
    /// <param name="numbers">Collection of product part numbers to search for</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>Collection of product details for all found products</returns>
    public async Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        // Execute a single database query using IN clause for better performance
        var products = await _readDbContext.Products
            .Where(x => numbers.Contains(x.Number))
            .ToListAsync(cancellationToken);

        // Transform all found products to DTOs (note: no name validation here as batch operations may need incomplete data)
        return products.Select(p => new ProductDetailsDto(
            Number: p.Number,
            Name: p.Name,
            EAN13: p.EAN13
        ));
    }
}