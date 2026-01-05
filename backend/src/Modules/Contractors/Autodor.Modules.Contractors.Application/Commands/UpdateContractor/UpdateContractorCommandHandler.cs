using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public class UpdateContractorCommandHandler(IContractorsDbContext context) 
    : IRequestHandler<UpdateContractorCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            return Error.NotFound(
                code: "Contractor.NotFound",
                description: $"Contractor with ID {request.Id} was not found");

        contractor.UpdateDetails(
            request.Name,
            new TaxId(request.NIP),
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}