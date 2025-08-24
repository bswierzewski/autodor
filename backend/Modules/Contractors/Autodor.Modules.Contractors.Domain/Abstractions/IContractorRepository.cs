using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;

namespace Autodor.Modules.Contractors.Domain.Abstractions;

public interface IContractorRepository
{
    Task<Contractor?> GetByIdAsync(ContractorId id);
    Task<Contractor?> GetByNIPAsync(TaxId nip);
    Task<IEnumerable<Contractor>> GetAllAsync();
    void Add(Contractor contractor);
    void Update(Contractor contractor);
    void Delete(Contractor contractor);
}