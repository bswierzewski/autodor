using Application.Common.Interfaces;

namespace Application.Orders.Commands.ExcludeOrder;

public class ExcludeOrderCommand : IRequest<Unit>
{
    public string OrderId { get; set; }
}

public class ExcludeOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<ExcludeOrderCommand, Unit>
{
    public async Task<Unit> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.ExcludedOrders.FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken: cancellationToken);

        if (order == null)
            await context.ExcludedOrders.AddAsync(new Domain.Entities.ExcludedOrder
            {
                OrderId = request.OrderId
            }, cancellationToken);
        else
            context.ExcludedOrders.Remove(order);

        await context.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}
