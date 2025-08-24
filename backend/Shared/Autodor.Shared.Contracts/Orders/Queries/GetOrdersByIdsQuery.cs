using MediatR;

namespace Autodor.Shared.Contracts.Orders.Queries;

public record GetOrdersByIdsDto(
    string OrderNumber,
    DateTime OrderDate,
    Guid ContractorId,
    IEnumerable<GetOrdersByIdsItemDto> Items
);

public record GetOrdersByIdsItemDto(
    string PartNumber,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record GetOrdersByIdsQuery(IEnumerable<string> OrderNumbers) : IRequest<IEnumerable<GetOrdersByIdsDto>>;