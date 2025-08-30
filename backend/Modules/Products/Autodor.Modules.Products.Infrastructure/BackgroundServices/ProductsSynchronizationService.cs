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
            var newProducts = await soapService.GetProductsAsync();
            var productsList = newProducts.ToList();

            if (!productsList.Any())
            {
                _logger.LogWarning("No products received from Polcar, keeping existing data.");
                return;
            }

            // Usuń wszystkie istniejące produkty i dodaj nowe w jednej transakcji
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await context.Products.ExecuteDeleteAsync();
                await context.Products.AddRangeAsync(productsList);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully synchronized {Count} products from Polcar.", productsList.Count);
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
}