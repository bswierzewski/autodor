using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using SharedKernel.Domain.Interfaces;

namespace Autodor.Modules.Contractors.Domain.Abstractions;

public interface IContractorRepository : IRepository<Contractor>
{
    Task<Contractor?> GetByIdAsync(ContractorId id);
    Task<Contractor?> GetByNIPAsync(TaxId nip);
    Task<IEnumerable<Contractor>> GetAllAsync();
}