using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.GetContractor;

public class GetContractorHandler
{
    [WolverineGet("/contractors/{id}")]
    [Tags("Contractors")]
    public static async Task<GetContractorResponse> Handle(
        Guid id,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {id} was not found");

        return new GetContractorResponse(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        );
    }
}
