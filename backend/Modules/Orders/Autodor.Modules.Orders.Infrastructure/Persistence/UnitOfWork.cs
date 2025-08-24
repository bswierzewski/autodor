using Autodor.Modules.Orders.Domain.Abstractions;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrdersDbContext _context;

    public UnitOfWork(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}