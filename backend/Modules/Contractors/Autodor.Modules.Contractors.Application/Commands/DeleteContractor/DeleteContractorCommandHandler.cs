using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors.Commands;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
{
    private readonly IContractorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractorCommandHandler(
        IContractorRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByIdAsync(new ContractorId(request.Id));

        if (contractor == null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        _repository.Delete(contractor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}