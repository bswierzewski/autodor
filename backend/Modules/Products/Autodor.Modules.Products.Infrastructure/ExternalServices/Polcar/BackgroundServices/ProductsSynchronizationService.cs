using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;

/// <summary>
/// Background service that synchronizes product data from the Polcar external system every 24 hours.
/// Handles complete product catalog updates including additions and deletions.
/// </summary>
public class ProductsSynchronizationService(
    IServiceProvider serviceProvider,
    ILogger<ProductsSynchronizationService> logger) : BackgroundService
{
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);

    /// <summary>
    /// Executes the background service, performing initial synchronization and then periodic updates every 24 hours.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service shutdown</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
    /// Synchronizes products from the Polcar external system with the local database.
    /// Compares existing products with new data and performs necessary additions and deletions.
    /// </summary>
    private async Task SynchronizeProductsAsync()
    {
        var taskName = nameof(ProductsSynchronizationService);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

            var taskState = await context.BackgroundTaskStates
                .FirstOrDefaultAsync(x => x.TaskName == taskName);

            var shouldExecute = taskState == null ||
                               DateTime.UtcNow - taskState.LastExecutedAt >= _syncInterval;

            if (!shouldExecute)
            {
                logger.LogInformation("Products synchronization skipped. Last execution: {LastExecution}, Next execution in: {TimeRemaining}",
                    taskState?.LastExecutedAt,
                    taskState != null ? _syncInterval - (DateTime.UtcNow - taskState.LastExecutedAt) : TimeSpan.Zero);
                return;
            }

            var soapService = scope.ServiceProvider.GetRequiredService<IPolcarProductService>();

            logger.LogInformation("Starting products synchronization from Polcar...");

            var newProducts = (await soapService.GetProductsAsync()).ToList();

            if (!newProducts.Any())
            {
                logger.LogWarning("No products received from Polcar, keeping existing data.");
                return;
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                logger.LogInformation("Loading existing PartNumbers for comparison...");
                
                var existingPartNumbers = await context.Products
                    .Select(p => p.Number)
                    .ToHashSetAsync();
                
                logger.LogInformation("Found {Count} existing products in database", existingPartNumbers.Count);

                var newPartNumbers = newProducts
                    .Select(p => p.Number)
                    .ToHashSet();

                logger.LogInformation("Comparing {NewCount} new products with {ExistingCount} existing PartNumbers...", 
                    newPartNumbers.Count, existingPartNumbers.Count);

                await DeleteProductsAsync(context, existingPartNumbers, newPartNumbers);
                await AddProductsAsync(context, newProducts, existingPartNumbers);

                await transaction.CommitAsync();

                var addedCount = newProducts.Count(p => !existingPartNumbers.Contains(p.Number));
                var deletedCount = existingPartNumbers.Except(newPartNumbers).Count();
                var unchangedCount = existingPartNumbers.Count - deletedCount;

                logger.LogInformation("Successfully synchronized products: {Added} added, {Deleted} deleted, {Unchanged} unchanged",
                addedCount, deletedCount, unchangedCount);

                if (taskState == null)
                {
                    taskState = new BackgroundTaskState
                    {
                        TaskName = taskName,
                        LastExecutedAt = DateTime.UtcNow
                    };
                    await context.BackgroundTaskStates.AddAsync(taskState);
                }
                else
                {
                    taskState.LastExecutedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Products synchronization completed and timestamp updated.");
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
    /// Deletes products that are no longer present in the external system.
    /// </summary>
    /// <param name="context">Database context for product operations</param>
    /// <param name="existingPartNumbers">Set of currently existing product numbers</param>
    /// <param name="newPartNumbers">Set of product numbers from external system</param>
    private async Task DeleteProductsAsync(ProductsDbContext context, HashSet<string> existingPartNumbers, HashSet<string> newPartNumbers)
    {
        var partNumbersToDelete = existingPartNumbers.Except(newPartNumbers).ToHashSet();
        
        if (!partNumbersToDelete.Any())
            return;

        await context.Products
            .Where(p => partNumbersToDelete.Contains(p.Number))
            .ExecuteDeleteAsync();
        
        logger.LogInformation("Deleted {Count} products", partNumbersToDelete.Count);
    }

    /// <summary>
    /// Adds new products from the external system that don't exist in the local database.
    /// </summary>
    /// <param name="context">Database context for product operations</param>
    /// <param name="newProducts">List of products from external system</param>
    /// <param name="existingPartNumbers">Set of currently existing product numbers</param>
    private async Task AddProductsAsync(ProductsDbContext context, List<Product> newProducts, HashSet<string> existingPartNumbers)
    {
        var productsToAdd = newProducts
            .Where(p => !existingPartNumbers.Contains(p.Number))
            .ToList();
        
        if (!productsToAdd.Any())
            return;

        await context.Products.AddRangeAsync(productsToAdd);
        await context.SaveChangesAsync();
        logger.LogInformation("Added {Count} new products", productsToAdd.Count);
    }
}