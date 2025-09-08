using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Orders.Domain.Aggregates;

/// <summary>
/// Represents an order that has been excluded from processing.
/// This aggregate root maintains the business rule that once an order is excluded,
/// it should not be processed through the normal order workflow.
/// </summary>
public class ExcludedOrder : AggregateRoot<int>
{
    /// <summary>
    /// Gets the unique identifier of the excluded order.
    /// This corresponds to the original order ID from the external system (e.g., Polcar).
    /// </summary>
    public string OrderId { get; private set; } = null!;

    /// <summary>
    /// Gets the timestamp when the order was excluded from processing.
    /// This provides an audit trail for exclusion decisions.
    /// </summary>
    public DateTimeOffset DateTime { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// Required for proper entity materialization from the database.
    /// </summary>
    private ExcludedOrder() { }

    /// <summary>
    /// Initializes a new instance of the ExcludedOrder aggregate.
    /// Creates an exclusion record for business decision tracking and audit purposes.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to exclude</param>
    /// <param name="dateTime">The timestamp when the exclusion decision was made</param>
    public ExcludedOrder(string orderId, DateTimeOffset dateTime)
    {
        // Store the original order identifier for reference and lookup
        OrderId = orderId;

        // Record the exclusion timestamp for audit and business tracking
        DateTime = dateTime;
    }
}