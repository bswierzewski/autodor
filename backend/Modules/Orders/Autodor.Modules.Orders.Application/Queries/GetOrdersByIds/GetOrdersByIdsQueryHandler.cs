using Autodor.Modules.Orders.Domain.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByIds;

public class GetOrdersByIdsQueryHandler : IRequestHandler<GetOrdersByIdsQuery, IEnumerable<GetOrdersByIdsDto>>
{
    private readonly IPolcarDistributorsSalesService _soapService;

    public GetOrdersByIdsQueryHandler(IPolcarDistributorsSalesService soapService)
    {
        _soapService = soapService;
    }

    public Task<IEnumerable<GetOrdersByIdsDto>> Handle(GetOrdersByIdsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}