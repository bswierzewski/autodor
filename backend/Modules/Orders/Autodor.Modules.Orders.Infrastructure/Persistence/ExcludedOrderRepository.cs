using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class ExcludedOrderRepository : IExcludedOrderRepository
{
    private readonly OrdersDbContext _context;

    public ExcludedOrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetExcludedOrderNumbersAsync()
    {
        return await _context.ExcludedOrders
            .Select(x => x.OrderNumber)
            .ToListAsync();
    }

    public void Add(ExcludedOrder excludedOrder)
    {
        _context.ExcludedOrders.Add(excludedOrder);
    }
}