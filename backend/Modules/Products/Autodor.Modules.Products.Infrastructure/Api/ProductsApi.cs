using MediatR;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Autodor.Modules.Products.Application.Queries.GetProduct;
using Autodor.Modules.Products.Application.Queries.GetProducts;

namespace Autodor.Modules.Products.Infrastructure.Api;

internal sealed class ProductsApi : IProductsApi
{
    private readonly ISender _sender;

    public ProductsApi(ISender sender)
    {
        _sender = sender;
    }

    public async Task<ProductDetailsDto?> GetProductByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        var query = new GetProductQuery(partNumber);
        var result = await _sender.Send(query, cancellationToken);
        
        if (result is null)
            return null;

        return new ProductDetailsDto(
            PartNumber: result.PartNumber,
            Name: result.Name,
            Ean: result.Ean
        );
    }

    public async Task<IEnumerable<ProductDetailsDto>> GetProductsByPartNumbersAsync(IEnumerable<string> partNumbers, CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(partNumbers);
        var results = await _sender.Send(query, cancellationToken);

        return results.Select(r => new ProductDetailsDto(
            PartNumber: r.PartNumber,
            Name: r.Name,
            Ean: r.Ean
        ));
    }
}