using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Contractors.Features.CreateContractor;

public static class CreateContractorHandler
{
    [Authorize]
    public static async Task<CreateContractorResponse> Handle(
        CreateContractorCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId(command.NIP),
            command.Name,
            new Address(command.Street, command.City, command.ZipCode),
            new Email(command.Email)
        );

        await dbContext.Contractors.AddAsync(contractor, ct);

        return new CreateContractorResponse(contractorId.Value);
    }
}
