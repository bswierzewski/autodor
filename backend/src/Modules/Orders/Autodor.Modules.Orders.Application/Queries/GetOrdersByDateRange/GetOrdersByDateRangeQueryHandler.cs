using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;

public sealed class GetOrdersByDateRangeQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrdersByDateRangeQuery, IEnumerable<Order>>
{
    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var orders = await ordersRepository.GetOrdersByDateRangeAsync(request.DateFrom, request.DateTo);
        return orders;
    }
}