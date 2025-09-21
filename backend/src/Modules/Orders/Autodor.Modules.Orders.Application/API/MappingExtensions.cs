using Autodor.Shared.Contracts.Orders.Dtos;

namespace Autodor.Modules.Orders.Application.API;

public static class MappingExtensions
{
    public static OrderDto ToDto(this Domain.Entities.Order order)
    {
        var contractorDto = new OrderContractorDto(
            order.Contractor.Number,
            order.Contractor.Name
        );

        var itemDtos = order.Items.Select(item => new OrderItemDto(
            item.OrderId,
            item.Number,
            item.Quantity,
            item.Price,
            item.VatRate
        ));

        return new OrderDto(
            order.Id,
            order.Number,
            order.EntryDate,
            contractorDto,
            itemDtos
        );
    }
}