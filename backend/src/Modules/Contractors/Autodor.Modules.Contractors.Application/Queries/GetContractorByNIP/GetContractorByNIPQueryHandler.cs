using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

public class GetContractorByNIPQueryHandler : IRequestHandler<GetContractorByNIPQuery, GetContractorByNIPDto>
{
    private readonly IContractorsDbContext _dbContext;

    public GetContractorByNIPQueryHandler(IContractorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetContractorByNIPDto> Handle(GetContractorByNIPQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _dbContext.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NIP == new TaxId(request.NIP), cancellationToken);

        if (contractor is null)
            throw new KeyNotFoundException($"Contractor with NIP {request.NIP} not found");

        return contractor.ToGetContractorByNIPDto();
    }
}