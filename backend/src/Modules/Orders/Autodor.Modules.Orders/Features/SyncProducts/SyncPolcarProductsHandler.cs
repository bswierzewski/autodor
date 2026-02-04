using Autodor.Modules.Orders.Abstractions.Integrations;
using Autodor.Modules.Orders.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.SyncProducts;

/// <summary>
/// Handler for synchronizing products from external SOAP API to cache.
/// </summary>
public class SyncPolcarProductsHandler
{
    [WolverinePost("/products/sync")]
    [Tags("Products")]
    public static async Task Handle(
        SyncPolcarProductsCommand command,
        IProductsService productsService,
        IProductsCache productsCache,
        ILogger<SyncPolcarProductsHandler> logger)
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
