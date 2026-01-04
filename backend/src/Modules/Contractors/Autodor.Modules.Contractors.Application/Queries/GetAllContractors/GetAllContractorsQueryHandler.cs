using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

public class GetAllContractorsQueryHandler(IContractorsDbContext dbContext) : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await dbContext.Contractors
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return contractors.Select(c => c.ToGetAllContractorsDto());
    }
}