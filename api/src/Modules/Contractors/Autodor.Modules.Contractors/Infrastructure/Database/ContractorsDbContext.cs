using Autodor.Modules.Contractors.Domain;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Database;

public class ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Module.Name.ToLowerInvariant());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContractorsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
