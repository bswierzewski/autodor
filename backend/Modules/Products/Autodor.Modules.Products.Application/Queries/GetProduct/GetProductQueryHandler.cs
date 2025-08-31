using Autodor.Modules.Products.Domain.Aggregates;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Products.Application.Queries.GetProduct;

public class GetProductQueryHandler(IRepository<Product> repository) 
    : IRequestHandler<GetProductQuery, GetProductDto?>
{
    public async Task<GetProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(x => x.PartNumber == request.PartNumber);
        
        if (product == null || string.IsNullOrEmpty(product.Name))
            return null;

        return new GetProductDto(product.PartNumber, product.Name, product.Ean);
    }
}