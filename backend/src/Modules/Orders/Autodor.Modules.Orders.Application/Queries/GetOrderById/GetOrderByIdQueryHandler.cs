using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrderByIdQuery, ErrorOr<Order>>
{
    public async Task<ErrorOr<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetOrderByIdAsync(request.OrderId);

        if (order is null)
            return Error.NotFound(
                code: "Order.NotFound",
                description: $"Order with ID '{request.OrderId}' was not found");

        return order;
    }
}
