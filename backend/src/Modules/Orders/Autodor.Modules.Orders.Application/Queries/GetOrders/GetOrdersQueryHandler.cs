using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(IOrdersRepository ordersRepository) : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
{
    public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        // Jeśli nie podano 'To', szukamy zamówień z jednego dnia (From)
        if (!request.To.HasValue)        
            return await ordersRepository.GetOrdersByDateAsync(request.From);        

        // Jeśli podano 'To', szukamy zamówień z zakresu od From do To
        return await ordersRepository.GetOrdersByDateRangeAsync(request.From, request.To.Value);
    }
}
