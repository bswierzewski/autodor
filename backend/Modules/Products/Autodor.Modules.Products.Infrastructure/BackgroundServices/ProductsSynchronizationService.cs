using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Infrastructure.Abstractions;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.BackgroundServices;

public class ProductsSynchronizationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProductsSynchronizationService> _logger;
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);

    public ProductsSynchronizationService(
        IServiceProvider serviceProvider,
        ILogger<ProductsSynchronizationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Pierwsza synchronizacja przy starcie aplikacji
        await SynchronizeProductsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_syncInterval, stoppingToken);
                await SynchronizeProductsAsync();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Products synchronization service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in products synchronization service.");
            }
        }
    }

    private async Task SynchronizeProductsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            var soapService = scope.ServiceProvider.GetRequiredService<IPolcarProductService>();

            _logger.LogInformation("Starting products synchronization from Polcar...");

            // Pobierz nowe produkty z SOAP
            var newProducts = (await soapService.GetProductsAsync()).ToList();

            if (!newProducts.Any())
            {
                _logger.LogWarning("No products received from Polcar, keeping existing data.");
                return;
            }

            // Porównanie na podstawie PartNumber i synchronizacja zmian
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Loading existing PartNumbers for comparison...");
                
                // Pobierz tylko PartNumbers z bazy (minimalizuj transfer danych)
                var existingPartNumbers = await context.Products
                    .Select(p => p.PartNumber)
                    .ToHashSetAsync();
                
                _logger.LogInformation("Found {Count} existing products in database", existingPartNumbers.Count);

                var newPartNumbers = newProducts
                    .Select(p => p.PartNumber)
                    .ToHashSet();

                _logger.LogInformation("Comparing {NewCount} new products with {ExistingCount} existing PartNumbers...", 
                    newPartNumbers.Count, existingPartNumbers.Count);

                await DeleteProductsAsync(context, existingPartNumbers, newPartNumbers);
                await AddProductsAsync(context, newProducts, existingPartNumbers);

                await transaction.CommitAsync();

                var addedCount = newProducts.Count(p => !existingPartNumbers.Contains(p.PartNumber));
                var deletedCount = existingPartNumbers.Except(newPartNumbers).Count();
                var unchangedCount = existingPartNumbers.Count - deletedCount;
                
                _logger.LogInformation("Successfully synchronized products: {Added} added, {Deleted} deleted, {Unchanged} unchanged",
                addedCount, deletedCount, unchangedCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to update products in database, rolling back transaction.");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize products from Polcar. Keeping existing data.");
        }
    }

    private async Task DeleteProductsAsync(ProductsDbContext context, HashSet<string> existingPartNumbers, HashSet<string> newPartNumbers)
    {
        // Produkty do usunięcia (PartNumbers które już nie istnieją w nowych danych)
        var partNumbersToDelete = existingPartNumbers.Except(newPartNumbers).ToHashSet();
        
        if (!partNumbersToDelete.Any())
            return;

        await context.Products
            .Where(p => partNumbersToDelete.Contains(p.PartNumber))
            .ExecuteDeleteAsync();
        
        _logger.LogInformation("Deleted {Count} products", partNumbersToDelete.Count);
    }

    private async Task AddProductsAsync(ProductsDbContext context, List<Product> newProducts, HashSet<string> existingPartNumbers)
    {
        // Produkty do dodania (nowe PartNumbers)
        var productsToAdd = newProducts
            .Where(p => !existingPartNumbers.Contains(p.PartNumber))
            .ToList();
        
        if (!productsToAdd.Any())
            return;

        await context.Products.AddRangeAsync(productsToAdd);
        await context.SaveChangesAsync();
        _logger.LogInformation("Added {Count} new products", productsToAdd.Count);
    }
}