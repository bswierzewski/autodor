using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand>
{
    private readonly IRepository<Contractor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractorCommandHandler(
        IRepository<Contractor> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByIdAsync(new ContractorId(request.Id));

        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        contractor.UpdateDetails(
            request.Name,
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        _repository.Update(contractor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}