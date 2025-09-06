using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.Queries.GetProduct;

public class GetProductQueryHandler(IProductsReadDbContext readDbContext)
    : IRequestHandler<GetProductQuery, GetProductDto?>
{
    public async Task<GetProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await readDbContext.Products
            .FirstOrDefaultAsync(x => x.PartNumber == request.PartNumber, cancellationToken);

        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        return new GetProductDto(product.PartNumber, product.Name, product.Ean);
    }
}