using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler : IRequestHandler<GetContractorQuery, GetContractorDto>
{
    private readonly IContractorsDbContext _dbContext;

    public GetContractorQueryHandler(IContractorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetContractorDto> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _dbContext.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new KeyNotFoundException($"Contractor with ID {request.Id} not found");

        return contractor.ToGetContractorDto();
    }
}