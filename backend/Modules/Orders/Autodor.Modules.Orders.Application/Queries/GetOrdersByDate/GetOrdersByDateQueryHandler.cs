using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;

public sealed class GetOrdersByDateQueryHandler : IRequestHandler<GetOrdersByDateQuery, IEnumerable<Order>>
{
    private readonly IOrdersRepository _ordersRepository;

    public GetOrdersByDateQueryHandler(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateQuery request, CancellationToken cancellationToken)
    {
        return await _ordersRepository.GetOrdersByDateAsync(request.Date);
    }
}