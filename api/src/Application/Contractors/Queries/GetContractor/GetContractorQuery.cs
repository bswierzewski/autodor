using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Contractors.Queries.GetContractor;

public class GetContractorQuery : IRequest<Contractor> 
{
    public int Id { get; set; }
}

public class GetContractorsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetContractorQuery, Contractor>
{
    public async Task<Contractor> Handle(GetContractorQuery request, CancellationToken cancellationToken)
        => await context.Contractors.FindAsync(request.Id, cancellationToken);
}