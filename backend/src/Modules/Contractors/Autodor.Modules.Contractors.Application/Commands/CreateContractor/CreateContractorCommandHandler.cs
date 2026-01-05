using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

public class CreateContractorCommandHandler(IContractorsDbContext context) : IRequestHandler<CreateContractorCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId(request.NIP),
            request.Name,
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        await context.Contractors.AddAsync(contractor, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return contractorId.Value;
    }
}