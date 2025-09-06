using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.Abstractions;

public interface IProductsWriteDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}