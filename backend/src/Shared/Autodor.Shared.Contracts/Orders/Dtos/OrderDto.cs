namespace Autodor.Shared.Contracts.Orders.Dtos;

public record OrderDto(
    string Id,
    string Number,
    DateTime EntryDate,
    OrderContractorDto Contractor,
    IEnumerable<OrderItemDto> Items
);

public record OrderContractorDto(
    string Number,
    string Name
);

public record OrderItemDto(
    string OrderId,
    string Number,
    int Quantity,
    decimal Price,
    decimal VatRate
);