using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Shared.Contracts.Orders.Commands;
using MediatR;
using SharedKernel.Domain.Interfaces;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public class ExcludeOrderCommandHandler : IRequestHandler<ExcludeOrderCommand, bool>
{
    private readonly IExcludedOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcludeOrderCommandHandler(
        IExcludedOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var excludedOrder = new ExcludedOrder(
            request.OrderNumber,
            request.Reason,
            DateTime.UtcNow);

        await _repository.AddAsync(excludedOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}