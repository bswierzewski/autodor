using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Domain.Abstractions;

public interface IPolcarOrdersService
{
    Task<IEnumerable<Order>> GetOrdersAsync(DateTime dateFrom, DateTime dateTo, Guid contractorId);
    Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<string> orderNumbers);
}