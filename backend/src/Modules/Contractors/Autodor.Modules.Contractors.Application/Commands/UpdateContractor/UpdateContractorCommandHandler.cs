using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public class UpdateContractorCommandHandler(IContractorsDbContext context) : IRequestHandler<UpdateContractorCommand, Unit>
{
    public async Task<Unit> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new KeyNotFoundException($"Contractor with ID {request.Id} not found");

        contractor.UpdateDetails(
            request.Name,
            new TaxId(request.NIP),
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        context.Contractors.Update(contractor);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}