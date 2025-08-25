using MediatR;

namespace Autodor.Shared.Domain.Common;

public interface IDomainEvent : INotification
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
}