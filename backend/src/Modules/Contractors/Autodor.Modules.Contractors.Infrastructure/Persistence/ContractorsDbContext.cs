using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Contractors module, implementing both read and write abstractions.
/// </summary>
public class ContractorsDbContext : DbContext, IContractorsWriteDbContext, IContractorsReadDbContext
{
    public DbSet<Contractor> Contractors { get; set; }

    IQueryable<Contractor> IContractorsReadDbContext.Contractors => Contractors.AsNoTracking();

    public ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractorsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}