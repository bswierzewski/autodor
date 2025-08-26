using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Repositories;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorRepository : BaseRepository<Contractor>, IContractorRepository
{
    private readonly ContractorsDbContext _context;

    public ContractorRepository(ContractorsDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Contractor?> GetByIdAsync(ContractorId id)
    {
        return await _context.Contractors.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contractor?> GetByNIPAsync(TaxId nip)
    {
        return await _context.Contractors.FirstOrDefaultAsync(c => c.NIP == nip);
    }

    public async Task<IEnumerable<Contractor>> GetAllAsync()
    {
        return await _context.Contractors.ToListAsync();
    }
}