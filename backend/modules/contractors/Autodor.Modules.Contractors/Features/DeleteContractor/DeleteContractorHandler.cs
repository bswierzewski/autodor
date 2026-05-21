using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public class DeleteContractorHandler
{
    public static async Task Handle(
        DeleteContractorCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(command.Id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {command.Id} was not found");

        dbContext.Contractors.Remove(contractor);
    }
}
