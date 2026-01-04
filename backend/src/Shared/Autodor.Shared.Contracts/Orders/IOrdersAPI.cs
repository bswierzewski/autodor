using Autodor.Shared.Contracts.Orders.Dtos;

namespace Autodor.Shared.Contracts.Orders;

public interface IOrdersAPI
{
    Task<IEnumerable<OrderDto>> GetOrdersByDatesAsync(IEnumerable<DateTime> dates, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetExcludedOrderIdsAsync(CancellationToken cancellationToken = default);
}