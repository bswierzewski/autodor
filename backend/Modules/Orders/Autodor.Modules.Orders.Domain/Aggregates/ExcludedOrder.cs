using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : AggregateRoot<Guid>
{
    public string Number { get; private set; } = null!;
    public DateTime DateTime { get; private set; }

    private ExcludedOrder() { } // EF Constructor

    public ExcludedOrder(string number, DateTime dateTime)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        DateTime = dateTime;
    }
}