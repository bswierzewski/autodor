using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.API;

internal sealed class ProductsAPI : IProductsAPI
{
    private readonly IProductsReadDbContext _readDbContext;

    public ProductsAPI(IProductsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<ProductDetailsDto?> GetProductByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var product = await _readDbContext.Products
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);

        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        return new ProductDetailsDto(
            Number: product.Number,
            Name: product.Name,
            EAN13: product.EAN13
        );
    }

    public async Task<IEnumerable<ProductDetailsDto>> GetProductsByNumbersAsync(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        var products = await _readDbContext.Products
            .Where(x => numbers.Contains(x.Number))
            .ToListAsync(cancellationToken);

        return products.Select(p => new ProductDetailsDto(
            Number: p.Number,
            Name: p.Name,
            EAN13: p.EAN13
        ));
    }
}