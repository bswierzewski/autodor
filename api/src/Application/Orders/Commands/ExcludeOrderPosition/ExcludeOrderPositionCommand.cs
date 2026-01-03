using Application.Common.Interfaces;

namespace Application.Orders.Commands.ExcludeOrderPosition;

public class ExcludeOrderPositionCommand : IRequest<Unit>
{
    public string OrderId { get; set; }
    public string PartNumber { get; set; }
}

public class ExcludeOrderPositionCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ExcludeOrderPositionCommand, Unit>
{
    public async Task<Unit> Handle(ExcludeOrderPositionCommand request, CancellationToken cancellationToken)
    {
        var position = await context.ExcludedOrderPositions
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId && x.PartNumber == request.PartNumber,
                cancellationToken: cancellationToken);

        if (position == null)
            await context.ExcludedOrderPositions.AddAsync(new Domain.Entities.ExcludedOrderPosition
            {
                OrderId = request.OrderId,
                PartNumber = request.PartNumber
            }, cancellationToken);
        else
            context.ExcludedOrderPositions.Remove(position);

        await context.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}
