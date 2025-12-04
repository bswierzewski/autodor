using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler : IRequestHandler<GetContractorQuery, Result<GetContractorDto>>
{
    private readonly IContractorsReadDbContext _readDbContext;

    public GetContractorQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetContractorDto>> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _readDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            return Result<GetContractorDto>.Failure("CONTRACTOR_NOT_FOUND", $"Contractor with ID {request.Id} not found");

        return Result<GetContractorDto>.Success(contractor.ToGetContractorDto());
    }
}