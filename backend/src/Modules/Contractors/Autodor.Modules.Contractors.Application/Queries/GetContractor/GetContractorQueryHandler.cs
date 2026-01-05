using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler(IContractorsDbContext dbContext) : IRequestHandler<GetContractorQuery, GetContractorDto>
{
    public async Task<GetContractorDto> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        Domain.Aggregates.Contractor? contractor = null;

        if (request.Id.HasValue)
        {
            contractor = await dbContext.Contractors
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id.Value), cancellationToken);

            if (contractor is null)
                throw new KeyNotFoundException($"Contractor with ID {request.Id} not found");
        }
        else if (!string.IsNullOrEmpty(request.NIP))
        {
            contractor = await dbContext.Contractors
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NIP == new TaxId(request.NIP), cancellationToken);

            if (contractor is null)
                throw new KeyNotFoundException($"Contractor with NIP {request.NIP} not found");
        }
        else
        {
            throw new ArgumentException("Either Id or NIP must be provided");
        }

        return contractor.ToGetContractorDto();
    }
}