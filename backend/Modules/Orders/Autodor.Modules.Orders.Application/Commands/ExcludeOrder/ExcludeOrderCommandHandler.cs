using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Interfaces;
using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public class ExcludeOrderCommandHandler : IRequestHandler<ExcludeOrderCommand, bool>
{
    private readonly IOrdersWriteDbContext _writeDbContext;

    public ExcludeOrderCommandHandler(IOrdersWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<bool> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var excludedOrder = new ExcludedOrder(
            request.OrderNumber,
            request.Reason,
            DateTime.UtcNow);

        await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}