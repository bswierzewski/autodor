using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Modules.Products.Application.API;

public sealed class ProductsAPI(IProductsRepository repository) : IProductsAPI
{
    public Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var product = repository.GetProductByNumber(number);

        if (product == null || string.IsNullOrEmpty(product.Name))
            return Task.FromResult<ProductDetailsDto?>(null);

        return Task.FromResult<ProductDetailsDto?>(product.ToDto());
    }

    public Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        var products = repository.GetProductsByNumbers(numbers);
        return Task.FromResult(products.Select(p => p.ToDto()));
    }
}