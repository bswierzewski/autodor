using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler(IContractorsDbContext context) : IRequestHandler<DeleteContractorCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            return Error.NotFound(
                code: "Contractor.NotFound",
                description: $"Contractor with ID {request.Id} was not found");

        context.Contractors.Remove(contractor);

        await context.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}