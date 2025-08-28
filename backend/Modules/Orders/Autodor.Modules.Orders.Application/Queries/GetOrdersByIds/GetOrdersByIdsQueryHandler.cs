using Autodor.Modules.Orders.Domain.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByIds;

public class GetOrdersByIdsQueryHandler : IRequestHandler<GetOrdersByIdsQuery, IEnumerable<GetOrdersByIdsDto>>
{
    private readonly IPolcarOrdersService _ordersService;

    public GetOrdersByIdsQueryHandler(IPolcarOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    public Task<IEnumerable<GetOrdersByIdsDto>> Handle(GetOrdersByIdsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}