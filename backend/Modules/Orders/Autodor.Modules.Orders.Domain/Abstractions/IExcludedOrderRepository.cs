using Autodor.Modules.Orders.Domain.Aggregates;
using SharedKernel.Domain.Interfaces;

namespace Autodor.Modules.Orders.Domain.Abstractions;

public interface IExcludedOrderRepository : IRepository<ExcludedOrder>
{
    Task<IEnumerable<string>> GetExcludedOrderNumbersAsync();
}