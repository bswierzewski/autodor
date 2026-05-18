using Autodor.Modules.Contractors.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public sealed class Factory : ModuleDbContextDesignTimeFactory<ContractorsDbContext> { }

public sealed class ContractorsDbContext(DbContextOptions<ContractorsDbContext> options)
    : ModuleDbContext<ContractorsDbContext>(options, Schema)
{
    public const string Schema = "contractors";

    public DbSet<Contractor> Contractors => Set<Contractor>();
}
