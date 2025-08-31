using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
{
    private readonly IRepository<Contractor> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractorCommandHandler(
        IRepository<Contractor> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByIdAsync(new ContractorId(request.Id));

        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        _repository.Remove(contractor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}