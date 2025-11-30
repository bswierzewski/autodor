using Shared.Abstractions.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

/// <summary>
/// Represents an order that has been excluded from processing or invoicing.
/// </summary>
public class ExcludedOrder : AggregateRoot<int>
{
    /// <summary>
    /// Gets the identifier of the excluded order.
    /// </summary>
    public string OrderId { get; private set; } = null!;

    /// <summary>
    /// Gets the date and time when the order was excluded.
    /// </summary>
    public DateTimeOffset DateTime { get; private set; }

    private ExcludedOrder() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludedOrder"/> class.
    /// </summary>
    /// <param name="orderId">The identifier of the order to exclude.</param>
    /// <param name="dateTime">The date and time when the exclusion occurs.</param>
    public ExcludedOrder(string orderId, DateTimeOffset dateTime)
    {
        OrderId = orderId;

        DateTime = dateTime;
    }
}