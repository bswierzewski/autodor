using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : DbContext(options), IContractorsDbContext
{
    public DbSet<Contractor> Contractors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractorsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}