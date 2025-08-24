using MediatR;

namespace Autodor.Modules.Invoicing.Domain.Common;

public interface IDomainEvent : INotification
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
}