using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.Queries.GetProducts;

public class GetProductsQueryHandler(IProductsReadDbContext readDbContext) 
    : IRequestHandler<GetProductsQuery, IEnumerable<GetProductsDto>>
{
    public async Task<IEnumerable<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await readDbContext.Products
            .Where(x => request.PartNumbers.Contains(x.PartNumber))
            .ToListAsync(cancellationToken);

        return products.Select(p => new GetProductsDto(p.PartNumber, p.Name, p.Ean));
    }
}