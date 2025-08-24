using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Shared.Contracts.Products.Queries;
using MediatR;

namespace Autodor.Modules.Products.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<GetProductsDto>>
{
    private readonly IPolcarProductsService _productsService;

    public GetProductsQueryHandler(IPolcarProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<IEnumerable<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productsService.GetProductsAsync(request.PartNumbers);
        return products.Select(p => new GetProductsDto(p.PartNumber, p.Name));
    }
}