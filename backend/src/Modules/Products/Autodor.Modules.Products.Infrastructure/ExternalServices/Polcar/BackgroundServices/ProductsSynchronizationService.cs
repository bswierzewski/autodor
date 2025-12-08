using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;

/// <summary>
/// Background service that synchronizes product data from the Polcar external system every 24 hours.
/// Loads products into in-memory repository for fast access.
/// </summary>
public class ProductsSynchronizationService(
    IServiceProvider serviceProvider,
    IHostEnvironment environment,
    ILogger<ProductsSynchronizationService> logger) : BackgroundService
{
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);
    private DateTime _lastExecutionTime = DateTime.MinValue;

    /// <summary>
    /// Executes the background service, performing initial synchronization and then periodic updates every 24 hours.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service shutdown</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!environment.IsProduction())
        {
            logger.LogInformation("Products synchronization service works only in Production environment.");
            return;
        }

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
    /// Synchronizes products from the Polcar external system and loads them into in-memory repository.
    /// </summary>
    private async Task SynchronizeProductsAsync()
    {
        try
        {
            var shouldExecute = _lastExecutionTime == DateTime.MinValue ||
                               DateTime.UtcNow - _lastExecutionTime >= _syncInterval;

            if (!shouldExecute)
            {
                logger.LogInformation("Products synchronization skipped. Last execution: {LastExecution}, Next execution in: {TimeRemaining}",
                    _lastExecutionTime,
                    _syncInterval - (DateTime.UtcNow - _lastExecutionTime));
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var soapService = scope.ServiceProvider.GetRequiredService<IPolcarProductService>();
            var repository = scope.ServiceProvider.GetRequiredService<IProductsRepository>();

            logger.LogInformation("Starting products synchronization from Polcar...");

            var newProducts = (await soapService.GetProductsAsync()).ToList();

            if (!newProducts.Any())
            {
                logger.LogWarning("No products received from Polcar, keeping existing data.");
                return;
            }

            var oldCount = repository.GetProductCount();

            repository.ReplaceAllProducts(newProducts);

            _lastExecutionTime = DateTime.UtcNow;

            logger.LogInformation("Successfully synchronized products: {NewCount} products loaded, replaced {OldCount} products",
                newProducts.Count, oldCount);
            logger.LogInformation("Products synchronization completed. Next execution at: {NextExecution}",
                _lastExecutionTime.Add(_syncInterval));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize products from Polcar. Keeping existing data.");
        }
    }
}