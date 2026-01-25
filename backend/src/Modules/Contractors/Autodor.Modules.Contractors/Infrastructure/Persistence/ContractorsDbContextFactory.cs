using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

public class ContractorsDbContextFactory : IDesignTimeDbContextFactory<ContractorsDbContext>
{
    public ContractorsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractorsDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=Autodor");

        return new ContractorsDbContext(optionsBuilder.Options);
    }
}
