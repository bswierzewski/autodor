using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler(IContractorsDbContext dbContext) : IRequestHandler<GetContractorQuery, ErrorOr<GetContractorDto>>
{
    public async Task<ErrorOr<GetContractorDto>> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        Domain.Aggregates.Contractor? contractor = null;

        if (request.Id.HasValue)
        {
            contractor = await dbContext.Contractors
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id.Value), cancellationToken);

            if (contractor is null)
                return Error.NotFound(
                    code: "Contractor.NotFound",
                    description: $"Contractor with ID {request.Id} was not found");
        }
        else if (!string.IsNullOrEmpty(request.NIP))
        {
            contractor = await dbContext.Contractors
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NIP == new TaxId(request.NIP), cancellationToken);

            if (contractor is null)
                return Error.NotFound(
                    code: "Contractor.NotFound",
                    description: $"Contractor with NIP {request.NIP} was not found");
        }
        else
        {
            return Error.Validation(
                code: "Contractor.InvalidQuery",
                description: "Either Id or NIP must be provided");
        }

        return contractor.ToGetContractorDto();
    }
}