using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public class ExcludeOrderCommandHandler(IOrdersWriteDbContext writeDbContext)
    : IRequestHandler<ExcludeOrderCommand, bool>
{
    private readonly IOrdersWriteDbContext _writeDbContext = writeDbContext;

    public async Task<bool> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var excludedOrder = new ExcludedOrder(
            request.Number,
            DateTime.UtcNow);

        await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}