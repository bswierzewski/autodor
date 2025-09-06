using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : AggregateRoot<Guid>
{
    public string OrderNumber { get; private set; } = null!;
    public string Reason { get; private set; } = null!;
    public DateTime ExcludedDate { get; private set; }

    private ExcludedOrder() { } // EF Constructor

    public ExcludedOrder(string orderNumber, string reason, DateTime excludedDate)
    {
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        ExcludedDate = excludedDate;
    }

    public void UpdateReason(string newReason)
    {
        Reason = newReason ?? throw new ArgumentNullException(nameof(newReason));
    }
}