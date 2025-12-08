using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Contractors module.
/// </summary>
public class ContractorsDbContext : DbContext, IContractorsDbContext
{
    public DbSet<Contractor> Contractors { get; set; }

    public ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractorsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}