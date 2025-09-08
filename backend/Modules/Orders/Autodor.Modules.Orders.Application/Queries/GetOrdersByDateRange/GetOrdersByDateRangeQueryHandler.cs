using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;

public sealed class GetOrdersByDateRangeQueryHandler : IRequestHandler<GetOrdersByDateRangeQuery, IEnumerable<Order>>
{
    private readonly IOrdersRepository _ordersRepository;

    public GetOrdersByDateRangeQueryHandler(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await _ordersRepository.GetOrdersByDateRangeAsync(request.DateFrom, request.DateTo);
    }
}