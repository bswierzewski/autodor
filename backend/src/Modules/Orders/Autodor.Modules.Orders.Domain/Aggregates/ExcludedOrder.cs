using BuildingBlocks.Abstractions.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : AggregateRoot<int>
{
    public string OrderId { get; private set; } = null!;

    public DateTimeOffset DateTime { get; private set; }

    private ExcludedOrder() { }

    public ExcludedOrder(string orderId, DateTimeOffset dateTime)
    {
        OrderId = orderId;

        DateTime = dateTime;
    }
}