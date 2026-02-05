using Autodor.Modules.Orders.Infrastructure.Integrations.Products;
using Autodor.Modules.Orders.Infrastructure.Services.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.SyncProducts;

public static class SyncProducts
{
    /// <summary>
    /// Command
    /// </summary>
    public record Command;

    /// <summary>
    /// Handler for synchronizing products from external SOAP API to cache.
    /// </summary>
    public static class Handler
    {
        [WolverinePost("/products/sync")]
        [Tags("Products")]
        public static async Task Handle(
            Command command,
            IProductsService productsService,
            IProductsCache productsCache,
            ILogger logger)
        {
            logger.LogInformation("Starting products synchronization from SOAP API");

            try
            {
                var products = await productsService.GetProductsAsync();

                productsCache.Set(products);

                logger.LogInformation("Products synchronization completed successfully. {Count} products synced", products.Count());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to synchronize products from SOAP API");
                throw;
            }
        }
    }
}
