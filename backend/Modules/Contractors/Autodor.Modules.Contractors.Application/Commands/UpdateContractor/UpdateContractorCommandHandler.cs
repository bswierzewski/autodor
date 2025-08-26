using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors.Commands;
using MediatR;
using SharedKernel.Domain.Interfaces;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand>
{
    private readonly IContractorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractorCommandHandler(
        IContractorRepository repository,
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