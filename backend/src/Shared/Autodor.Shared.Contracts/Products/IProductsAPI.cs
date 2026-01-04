using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Shared.Contracts.Products;

public interface IProductsAPI
{
    Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default);
}