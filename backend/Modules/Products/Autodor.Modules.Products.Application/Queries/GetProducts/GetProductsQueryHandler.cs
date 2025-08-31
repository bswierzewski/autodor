using Autodor.Modules.Products.Domain.Aggregates;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Products.Application.Queries.GetProducts;

public class GetProductsQueryHandler(IRepository<Product> repository) 
    : IRequestHandler<GetProductsQuery, IEnumerable<GetProductsDto>>
{
    public async Task<IEnumerable<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.FindAsync(x => request.PartNumbers.Contains(x.PartNumber));

        return products.Select(p => new GetProductsDto(p.PartNumber, p.Name, p.Ean));
    }
}