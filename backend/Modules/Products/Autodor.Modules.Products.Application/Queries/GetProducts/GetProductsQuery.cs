using MediatR;

namespace Autodor.Modules.Products.Application.Queries.GetProducts;

public record GetProductsDto(
    string PartNumber,
    string Name,
    string Ean
);

public record GetProductsQuery(IEnumerable<string> PartNumbers) : IRequest<IEnumerable<GetProductsDto>>;