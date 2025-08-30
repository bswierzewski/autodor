using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Domain.ValueObjects;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ProductsDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Product> GetProductAsync(string partNumber)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.PartNumber == partNumber);

        if (product == null)
        {
            _logger.LogError("Product with part number '{PartNumber}' not found", partNumber);
            return Product.Empty;
        }

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(IEnumerable<string> partNumbers)
    {
        var partNumbersList = partNumbers.ToList();

        return await _context.Products
            .Where(p => partNumbersList.Contains(p.PartNumber))
            .ToListAsync();
    }
}