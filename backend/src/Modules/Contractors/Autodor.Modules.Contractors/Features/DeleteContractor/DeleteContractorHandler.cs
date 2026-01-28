using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public class DeleteContractorHandler
{
    [WolverineDelete("/contractors/{id}")]
    [Tags("Contractors")]
    public static async Task Handle(
        Guid id,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {id} was not found");

        dbContext.Contractors.Remove(contractor);
        await dbContext.SaveChangesAsync(ct);
    }
}
