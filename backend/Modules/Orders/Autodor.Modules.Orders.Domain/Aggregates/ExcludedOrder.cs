using Autodor.Modules.Orders.Domain.Common;

namespace Autodor.Modules.Orders.Domain.Aggregates;

public class ExcludedOrder : AggregateRoot<Guid>
{
    public string OrderNumber { get; private set; }
    public string Reason { get; private set; }
    public DateTime ExcludedDate { get; private set; }

    private ExcludedOrder() : base(Guid.Empty) { } // EF Constructor

    public ExcludedOrder(string orderNumber, string reason, DateTime excludedDate) : base(Guid.NewGuid())
    {
        OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        ExcludedDate = excludedDate;
    }

    public void UpdateReason(string newReason)
    {
        Reason = newReason ?? throw new ArgumentNullException(nameof(newReason));
        SetModifiedDate();
    }
}