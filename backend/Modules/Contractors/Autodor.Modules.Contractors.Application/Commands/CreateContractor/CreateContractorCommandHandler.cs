using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, Guid>
{
    private readonly IRepository<Contractor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContractorCommandHandler(
        IRepository<Contractor> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractorId = new ContractorId(Guid.NewGuid());
        var contractor = new Contractor(
            contractorId,
            new TaxId(request.NIP),
            request.Name,
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        await _repository.AddAsync(contractor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return contractorId.Value;
    }
}