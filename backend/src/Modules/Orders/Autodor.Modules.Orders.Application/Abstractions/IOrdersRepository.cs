using Autodor.Modules.Orders.Domain.Entities;

namespace Autodor.Modules.Orders.Application.Abstractions;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);   
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
    Task<Order?> GetOrderByIdAsync(string orderId);
    Task<Order?> GetOrderByIdAndDateAsync(string orderId, DateTime date);
}