using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public class UpdateContractorHandler
{
    [WolverinePut("/contractors/{id}")]
    [Tags("Contractors")]
    [EndpointName("UpdateContractor")]
    [EndpointSummary("Update contractor details")]
    public static async Task Handle(
        Guid id,
        UpdateContractorCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {id} was not found");

        contractor.UpdateDetails(
            command.Name,
            new TaxId(command.NIP),
            new Address(command.Street, command.City, command.ZipCode),
            new Email(command.Email)
        );
    }
}
