using MediatR;

namespace Autodor.Shared.Contracts.Products.Queries;

public record GetProductDto(
    string PartNumber,
    string Name
);

public record GetProductQuery(string PartNumber) : IRequest<GetProductDto?>;