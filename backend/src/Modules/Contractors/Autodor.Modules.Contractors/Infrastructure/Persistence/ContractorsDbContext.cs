using Microsoft.EntityFrameworkCore;
using Autodor.Modules.Contractors.Domain.Aggregates;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : DbContext(options)
{
    public DbSet<Contractor> Contractors => Set<Contractor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractorsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
