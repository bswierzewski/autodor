using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrderByIdQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetOrderByIdAsync(request.OrderId);
        return order;
    }
}
