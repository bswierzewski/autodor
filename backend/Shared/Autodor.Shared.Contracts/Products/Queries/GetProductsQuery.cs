using MediatR;

namespace Autodor.Shared.Contracts.Products.Queries;

public record GetProductsDto(
    string PartNumber,
    string Name
);

public record GetProductsQuery(IEnumerable<string> PartNumbers) : IRequest<IEnumerable<GetProductsDto>>;