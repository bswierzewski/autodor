using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;

/// <summary>
/// Background service responsible for synchronizing product data from Polcar external service.
/// Runs periodic synchronization to keep the local product database up-to-date with supplier changes.
/// </summary>
public class ProductsSynchronizationService(
    IServiceProvider serviceProvider,
    ILogger<ProductsSynchronizationService> logger) : BackgroundService
{
    /// <summary>
    /// Interval between synchronization operations. Set to 24 hours to balance data freshness with API load.
    /// </summary>
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);

    /// <summary>
    /// Main execution loop for the background service.
    /// Performs initial synchronization on startup, then continues with periodic updates.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service shutdown</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Perform initial synchronization on application startup to ensure fresh data availability
        await SynchronizeProductsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for the configured interval before next synchronization
                await Task.Delay(_syncInterval, stoppingToken);
                await SynchronizeProductsAsync();
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Products synchronization service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in products synchronization service.");
            }
        }
    }

    /// <summary>
    /// Performs the actual synchronization of products from external service to local database.
    /// Uses efficient bulk operations and transactional integrity to maintain data consistency.
    /// </summary>
    private async Task SynchronizeProductsAsync()
    {
        try
        {
            // Create a new service scope for this operation to ensure proper DI lifetime management
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            var soapService = scope.ServiceProvider.GetRequiredService<IPolcarProductService>();

            logger.LogInformation("Starting products synchronization from Polcar...");

            // Fetch fresh product data from external Polcar service
            var newProducts = (await soapService.GetProductsAsync()).ToList();

            // Business rule: If external service returns no data, preserve existing database state
            // This prevents accidental data loss due to temporary service issues
            if (!newProducts.Any())
            {
                logger.LogWarning("No products received from Polcar, keeping existing data.");
                return;
            }

            // Execute synchronization within transaction to ensure atomicity
            // Compare based on PartNumber and synchronize changes
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                logger.LogInformation("Loading existing PartNumbers for comparison...");
                
                // Load only PartNumbers from database to minimize data transfer and memory usage
                // Business optimization: We only need identifiers for comparison, not full product data
                var existingPartNumbers = await context.Products
                    .Select(p => p.Number)
                    .ToHashSetAsync();
                
                logger.LogInformation("Found {Count} existing products in database", existingPartNumbers.Count);

                // Extract PartNumbers from new data for efficient set operations
                var newPartNumbers = newProducts
                    .Select(p => p.Number)
                    .ToHashSet();

                logger.LogInformation("Comparing {NewCount} new products with {ExistingCount} existing PartNumbers...", 
                    newPartNumbers.Count, existingPartNumbers.Count);

                // Execute synchronization operations in sequence for data consistency
                await DeleteProductsAsync(context, existingPartNumbers, newPartNumbers);
                await AddProductsAsync(context, newProducts, existingPartNumbers);

                await transaction.CommitAsync();

                var addedCount = newProducts.Count(p => !existingPartNumbers.Contains(p.Number));
                var deletedCount = existingPartNumbers.Except(newPartNumbers).Count();
                var unchangedCount = existingPartNumbers.Count - deletedCount;
                
                logger.LogInformation("Successfully synchronized products: {Added} added, {Deleted} deleted, {Unchanged} unchanged",
                addedCount, deletedCount, unchangedCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Failed to update products in database, rolling back transaction.");
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize products from Polcar. Keeping existing data.");
        }
    }

    /// <summary>
    /// Removes products from the database that are no longer present in the external data source.
    /// Uses efficient bulk delete operations to minimize database operations.
    /// </summary>
    /// <param name="context">Database context for executing operations</param>
    /// <param name="existingPartNumbers">Set of part numbers currently in the database</param>
    /// <param name="newPartNumbers">Set of part numbers from the external source</param>
    private async Task DeleteProductsAsync(ProductsDbContext context, HashSet<string> existingPartNumbers, HashSet<string> newPartNumbers)
    {
        // Identify products to delete: those that exist locally but not in the new external data
        // Business rule: Remove discontinued products to maintain data accuracy
        var partNumbersToDelete = existingPartNumbers.Except(newPartNumbers).ToHashSet();
        
        if (!partNumbersToDelete.Any())
            return;

        // Use bulk delete for performance - avoids loading entities into memory
        await context.Products
            .Where(p => partNumbersToDelete.Contains(p.Number))
            .ExecuteDeleteAsync();
        
        logger.LogInformation("Deleted {Count} products", partNumbersToDelete.Count);
    }

    /// <summary>
    /// Adds new products to the database that are present in external data but not locally.
    /// Uses bulk insert operations for optimal performance with large datasets.
    /// </summary>
    /// <param name="context">Database context for executing operations</param>
    /// <param name="newProducts">Complete list of products from external source</param>
    /// <param name="existingPartNumbers">Set of part numbers currently in the database</param>
    private async Task AddProductsAsync(ProductsDbContext context, List<Product> newProducts, HashSet<string> existingPartNumbers)
    {
        // Identify products to add: those present in external data but not in local database
        // Business rule: Add all new products to expand catalog coverage
        var productsToAdd = newProducts
            .Where(p => !existingPartNumbers.Contains(p.Number))
            .ToList();
        
        if (!productsToAdd.Any())
            return;

        // Use bulk insert for performance with large product catalogs
        await context.Products.AddRangeAsync(productsToAdd);
        await context.SaveChangesAsync();
        logger.LogInformation("Added {Count} new products", productsToAdd.Count);
    }
}