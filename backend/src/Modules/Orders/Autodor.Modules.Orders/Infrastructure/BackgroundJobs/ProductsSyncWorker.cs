using Autodor.Modules.Orders.Features.SyncProducts;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Autodor.Modules.Orders.Infrastructure.BackgroundJobs;

/// <summary>
/// Background worker that loads products into cache on application startup (hot load).
/// </summary>
public class ProductsSyncWorker(
    IServiceScopeFactory scopeFactory,
    IHostApplicationLifetime lifetime,
    IHostEnvironment environment,
    ILogger<ProductsSyncWorker> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await lifetime.WaitForAppStartupAsync(stoppingToken)) return;

        if (!environment.IsProduction())
        {
            logger.LogInformation($"{nameof(ProductsSyncWorker)} is disabled in non-production environment.");
            return;
        }

        logger.LogInformation($"{nameof(ProductsSyncWorker)} started.");

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            await messageBus.InvokeAsync(new SyncProducts.Command());

            logger.LogInformation("Products loaded into cache successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load products into cache on startup");
        }
    }
}