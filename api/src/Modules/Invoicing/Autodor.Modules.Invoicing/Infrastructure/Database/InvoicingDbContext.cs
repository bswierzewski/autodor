using Autodor.Modules.Invoicing.Domain;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Invoicing.Infrastructure.Database;

public class InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Module.Name.ToLowerInvariant());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoicingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
