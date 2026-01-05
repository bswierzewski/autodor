using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractors;

public class GetContractorsQueryHandler(IContractorsDbContext dbContext) : IRequestHandler<GetContractorsQuery, ErrorOr<IEnumerable<GetContractorsDto>>>
{
    public async Task<ErrorOr<IEnumerable<GetContractorsDto>>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Contractors
            .AsNoTracking()
            .Select(c => c.ToGetContractorsDto())
            .ToListAsync(cancellationToken);
    }
}