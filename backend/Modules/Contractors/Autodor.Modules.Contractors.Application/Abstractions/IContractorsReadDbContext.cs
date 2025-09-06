using Autodor.Modules.Contractors.Domain.Aggregates;

namespace Autodor.Modules.Contractors.Application.Abstractions;

public interface IContractorsReadDbContext
{
    IQueryable<Contractor> Contractors { get; }
}