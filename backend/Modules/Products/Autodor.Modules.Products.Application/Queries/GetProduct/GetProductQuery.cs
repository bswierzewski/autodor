using MediatR;

namespace Autodor.Modules.Products.Application.Queries.GetProduct;

public record GetProductDto(
    string PartNumber,
    string Name
);

public record GetProductQuery(string PartNumber) : IRequest<GetProductDto?>;