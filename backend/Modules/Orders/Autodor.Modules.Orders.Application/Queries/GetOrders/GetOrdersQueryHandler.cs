using Autodor.Modules.Orders.Domain.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<GetOrdersDto>>
{
    private readonly IPolcarOrdersService _soapService;

    public GetOrdersQueryHandler(IPolcarOrdersService soapService)
    {
        _soapService = soapService;
    }

    public Task<IEnumerable<GetOrdersDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}