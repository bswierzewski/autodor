using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Shared.Contracts.Products;

public interface IProductsAPI
{
    /// <summary>
    /// Gets product details by number (e.g., "007935016720")
    /// </summary>
    Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets multiple products by their numbers
    /// </summary>
    Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default);
}