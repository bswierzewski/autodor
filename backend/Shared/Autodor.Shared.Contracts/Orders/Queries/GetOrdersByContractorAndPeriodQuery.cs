using MediatR;

namespace Autodor.Shared.Contracts.Orders.Queries;

public record GetOrdersByContractorAndPeriodDto(
    string OrderNumber,
    DateTime OrderDate,
    Guid ContractorId,
    IEnumerable<GetOrdersByContractorAndPeriodItemDto> Items
);

public record GetOrdersByContractorAndPeriodItemDto(
    string PartNumber,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record GetOrdersByContractorAndPeriodQuery(Guid ContractorId, DateTime DateFrom, DateTime DateTo) : IRequest<IEnumerable<GetOrdersByContractorAndPeriodDto>>;