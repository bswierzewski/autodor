using BuildingBlocks.Core.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : Entity<string>
{
    private ExcludedOrder() { }

    public ExcludedOrder(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        Id = id;
    }
}
