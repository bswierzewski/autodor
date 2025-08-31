using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Shared.Contracts.Products;

public interface IProductsApi
{
    /// <summary>
    /// Gets product details by part number (e.g., "007935016720")
    /// </summary>
    Task<ProductDetailsDto?> GetProductByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets multiple products by their part numbers
    /// </summary>
    Task<IEnumerable<ProductDetailsDto>> GetProductsByPartNumbersAsync(IEnumerable<string> partNumbers, CancellationToken cancellationToken = default);
}