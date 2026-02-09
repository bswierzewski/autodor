using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Modules.Orders.Infrastructure.Services.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wolverine.Http;

namespace Autodor.Modules.Orders.Features.SyncProducts;

public static class SyncProductsHandler
{
    [WolverinePost("/products/sync")]
    [Tags("Products")]
    public static async Task Handle(
        SyncProductsCommand command,
        IProductsClient productsService,
        IProductsCache productsCache,
        ILogger logger)
    {
        logger.LogInformation("Starting products synchronization from SOAP API");

        try
        {
            // Fetch products from external API (returns DTO)
            var productDtos = await productsService.GetProductsAsync();

            // Map DTO to domain model
            var products = productDtos
                .Select(dto => new Product(
                    dto.Number,
                    dto.Name,
                    dto.EAN13
                ))
                .ToList();

            // Store domain models in cache
            productsCache.Set(products);

            logger.LogInformation("Products synchronization completed successfully. {Count} products synced", products.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize products from SOAP API");
            throw;
        }
    }
}
