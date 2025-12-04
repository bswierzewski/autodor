using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

public class GetContractorByNIPQueryHandler : IRequestHandler<GetContractorByNIPQuery, Result<GetContractorByNIPDto>>
{
    private readonly IContractorsReadDbContext _readDbContext;

    public GetContractorByNIPQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetContractorByNIPDto>> Handle(GetContractorByNIPQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _readDbContext.Contractors
            .FirstOrDefaultAsync(x => x.NIP == new TaxId(request.NIP), cancellationToken);

        if (contractor is null)
            return Result<GetContractorByNIPDto>.Failure("CONTRACTOR_NOT_FOUND", $"Contractor with NIP {request.NIP} not found");

        return Result<GetContractorByNIPDto>.Success(contractor.ToGetContractorByNIPDto());
    }
}