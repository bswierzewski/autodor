using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Domain.Abstractions;

public interface IExcludedOrderRepository
{
    Task<IEnumerable<string>> GetExcludedOrderNumbersAsync();
    void Add(ExcludedOrder excludedOrder);
}