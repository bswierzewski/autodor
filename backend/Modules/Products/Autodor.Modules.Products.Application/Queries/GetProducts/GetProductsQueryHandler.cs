using Autodor.Modules.Products.Domain.Abstractions;
using MediatR;

namespace Autodor.Modules.Products.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<GetProductsDto>>
{
    private readonly IProductRepository _productsService;

    public GetProductsQueryHandler(IProductRepository productsService)
    {
        _productsService = productsService;
    }

    public async Task<IEnumerable<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productsService.GetProductsAsync(request.PartNumbers);
        return products.Select(p => new GetProductsDto(p.PartNumber, p.Name, p.Ean));
    }
}