using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Repositories;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class ExcludedOrderRepository : BaseRepository<ExcludedOrder>, IExcludedOrderRepository
{
    private readonly OrdersDbContext _context;

    public ExcludedOrderRepository(OrdersDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetExcludedOrderNumbersAsync()
    {
        return await _context.ExcludedOrders
            .Select(x => x.OrderNumber)
            .ToListAsync();
    }
}