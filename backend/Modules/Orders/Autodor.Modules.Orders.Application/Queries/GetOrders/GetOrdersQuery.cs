using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrders;

public record GetOrdersDto(
    string OrderNumber,
    DateTime OrderDate,
    Guid ContractorId,
    IEnumerable<GetOrdersItemDto> Items
);

public record GetOrdersItemDto(
    string PartNumber,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record GetOrdersQuery(DateTime DateFrom, DateTime DateTo, Guid ContractorId) : IRequest<IEnumerable<GetOrdersDto>>;