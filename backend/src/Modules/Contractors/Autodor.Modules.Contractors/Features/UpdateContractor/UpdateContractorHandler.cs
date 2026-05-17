using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public class UpdateContractorHandler
{
    [WolverinePut("/api/contractors/{id}")]
    [Tags("Contractors")]
    [EndpointName("UpdateContractor")]
    [EndpointSummary("Update contractor details")]
    public static async Task Handle(
        [AsParameters] UpdateContractorRequest request,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractor = await dbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), ct);

        if (contractor is null)
            throw new NotFoundException($"Contractor with ID {request.Id} was not found");

        contractor.UpdateDetails(
            request.Command!.Name,
            new TaxId(request.Command.NIP),
            new Address(request.Command.Street, request.Command.City, request.Command.ZipCode),
            new Email(request.Command.Email)
        );
    }
}
