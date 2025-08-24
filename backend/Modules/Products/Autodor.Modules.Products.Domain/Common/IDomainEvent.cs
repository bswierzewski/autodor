using MediatR;

namespace Autodor.Modules.Products.Domain.Common;

public interface IDomainEvent : INotification
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
}