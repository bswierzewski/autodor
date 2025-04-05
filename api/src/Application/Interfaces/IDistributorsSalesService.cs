using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDistributorsSalesService
{
    Task<IEnumerable<Order>> GetOrdersAsync(DateTime date);
}
