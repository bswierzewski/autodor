using BuildingBlocks.Kernel.Abstractions;
using BuildingBlocks.Kernel.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : Entity<string>, IAggregateRoot
{
    private ExcludedOrder() { }

    public ExcludedOrder(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        Id = id;
    }
}
