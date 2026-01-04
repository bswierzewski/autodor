using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler(IContractorsDbContext context) : IRequestHandler<DeleteContractorCommand, Unit>
{
    public async Task<Unit> Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new KeyNotFoundException($"Contractor with ID {request.Id} not found");

        context.Contractors.Remove(contractor);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}