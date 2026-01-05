using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrdersQuery, ErrorOr<IEnumerable<Order>>>
{
    public async Task<ErrorOr<IEnumerable<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Order> orders;

        if (request.To.HasValue)
            orders = await ordersRepository.GetOrdersByDateRangeAsync(request.From, request.To.Value);
        else
            orders = await ordersRepository.GetOrdersByDateAsync(request.From);

        return orders.ToErrorOr();
    }
}