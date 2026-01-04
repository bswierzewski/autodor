using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public class ExcludeOrderCommandHandler(IOrdersDbContext context, TimeProvider timeProvider)
    : IRequestHandler<ExcludeOrderCommand, bool>
{
    private readonly IOrdersDbContext _context = context;

    public async Task<bool> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var excludedOrder = new ExcludedOrder(request.OrderId, timeProvider.GetUtcNow());

        await _context.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}