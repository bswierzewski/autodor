using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractor;

public static class GetContractorHandler
{
    [Authorize]
    public static async Task<GetContractorResponse> Handle(
        GetContractorCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(command.Id), ct);

        if (contractor is null)
            throw new NotFoundException($"Nie znaleziono kontrahenta o identyfikatorze {command.Id}.");

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
