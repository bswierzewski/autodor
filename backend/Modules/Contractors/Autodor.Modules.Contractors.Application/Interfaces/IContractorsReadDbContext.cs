using Autodor.Modules.Contractors.Domain.Aggregates;

namespace Autodor.Modules.Contractors.Application.Interfaces;

public interface IContractorsReadDbContext
{
    IQueryable<Contractor> Contractors { get; }
}