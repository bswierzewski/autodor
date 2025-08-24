using Autodor.Modules.Contractors.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContractorsDbContext _context;

    public UnitOfWork(ContractorsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}