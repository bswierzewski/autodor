using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Application.Abstractions;

public interface IOrdersReadDbContext
{
    IQueryable<ExcludedOrder> ExcludedOrders { get; }
}