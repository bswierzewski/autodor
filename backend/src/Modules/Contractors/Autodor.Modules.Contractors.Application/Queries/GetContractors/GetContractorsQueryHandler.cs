using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractors;

public class GetContractorsQueryHandler(IContractorsDbContext dbContext) : IRequestHandler<GetContractorsQuery, IEnumerable<GetContractorsDto>>
{
    public async Task<IEnumerable<GetContractorsDto>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await dbContext.Contractors
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return contractors.Select(c => c.ToGetContractorsDto());
    }
}