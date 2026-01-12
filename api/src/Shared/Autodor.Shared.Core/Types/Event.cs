using Autodor.Shared.Core.Interfaces;

namespace Autodor.Shared.Core.Types;

/// <summary>
/// Base class for domain events with automatic ID and timestamp generation
/// </summary>
public abstract class Event : IEvent
{
    /// <summary>Unique identifier automatically generated when event is created</summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>Timestamp set to UTC now when event is created</summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}