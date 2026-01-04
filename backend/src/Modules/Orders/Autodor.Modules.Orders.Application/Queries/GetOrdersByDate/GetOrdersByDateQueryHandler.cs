using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;

public sealed class GetOrdersByDateQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrdersByDateQuery, IEnumerable<Order>>
{
    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateQuery request, CancellationToken cancellationToken)
    {
        var orders = await ordersRepository.GetOrdersByDateAsync(request.Date);
        return orders;
    }
}