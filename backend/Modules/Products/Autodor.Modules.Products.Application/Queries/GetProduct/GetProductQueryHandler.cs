using Autodor.Modules.Products.Domain.Abstractions;
using MediatR;

namespace Autodor.Modules.Products.Application.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, GetProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductAsync(request.PartNumber);
        
        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        return new GetProductDto(product.PartNumber, product.Name);
    }
}