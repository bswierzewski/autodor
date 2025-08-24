using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorRepository : IContractorRepository
{
    private readonly ContractorsDbContext _context;

    public ContractorRepository(ContractorsDbContext context)
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

    public void Add(Contractor contractor)
    {
        _context.Contractors.Add(contractor);
    }

    public void Update(Contractor contractor)
    {
        _context.Contractors.Update(contractor);
    }

    public void Delete(Contractor contractor)
    {
        _context.Contractors.Remove(contractor);
    }
}