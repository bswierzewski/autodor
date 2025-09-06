using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorsDbContext : DbContext, IContractorsWriteDbContext, IContractorsReadDbContext
{
    public DbSet<Contractor> Contractors { get; set; }
    
    IQueryable<Contractor> IContractorsReadDbContext.Contractors => Contractors.AsNoTracking();

    public ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : base(options)
    {
        // Interceptory są automatycznie rejestrowane przez AddInterceptors w DI
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContractorConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}