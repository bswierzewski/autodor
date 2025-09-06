using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Application.Interfaces;

public interface IOrdersReadDbContext
{
    IQueryable<ExcludedOrder> ExcludedOrders { get; }
}