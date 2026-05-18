using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public class DeleteContractorHandler
{
    [WolverineDelete("/api/contractors/{id}")]
    [Tags("Contractors")]
    [EndpointName("DeleteContractor")]
    [EndpointSummary("Delete contractor")]
    public static async Task Handle(
        Guid id,
        [FromServices] ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {id} was not found");

        dbContext.Contractors.Remove(contractor);
    }
}
