using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Contractors.Queries.GetContractors;

public class GetContractorsQuery : IRequest<IEnumerable<Contractor>> { }

public class GetContractorsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetContractorsQuery, IEnumerable<Contractor>>
{
    public async Task<IEnumerable<Contractor>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
        => await context.Contractors.ToListAsync(cancellationToken: cancellationToken);
}