using System.Collections.Frozen;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference.Models;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Soap;
using BuildingBlocks.Soap.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsClient(
    IOptions<ProductsOptions> options,
    SoapPipeline<ProductsSoapClient> soapPipeline,
    IMemoryCache memoryCache,
    ILogger<ProductsClient> logger
    ) : IProductsClient
{
    private const string ProductsCacheKey = "orders:products:dictionary";
    private static readonly TimeSpan ProductsCacheDuration = TimeSpan.FromDays(2);
    private static readonly SemaphoreSlim CacheLock = new(1, 1);

    private readonly ProductsOptions _options = options.Value;

    public async Task<FrozenDictionary<string, Models.Product>> GetProductsAsync()
    {
        if (memoryCache.TryGetValue<FrozenDictionary<string, Models.Product>>(ProductsCacheKey, out var cachedProducts))
            return cachedProducts!;

        await CacheLock.WaitAsync();

        try
        {
            if (memoryCache.TryGetValue(ProductsCacheKey, out cachedProducts))
                return cachedProducts!;

            var response = await soapPipeline.InvokeAsync(
                async client =>
                {
                    return await client.GetEAN13ListAsync(
                        Login: _options.Login,
                        Password: _options.Password,
                        LanguageID: _options.LanguageId,
                        FormatID: _options.FormatId);
                },
                SoapCallContext.ForOperation(nameof(GetProductsAsync)));

            var xml = response.Body?.GetEAN13ListResult?.OuterXml;

            if (string.IsNullOrWhiteSpace(xml))
            {
                logger.LogWarning("Products SOAP response did not contain XML payload.");
                return FrozenDictionary<string, Models.Product>.Empty;
            }

            var products = xml.FromXml<ProductRoot>()
                ?.Items
                ?.Select(xml => new Models.Product
                {
                    Number = xml.Number,
                    Name = xml.PartName,
                    EAN13 = xml.EAN13Code
                })
                .GroupBy(p => p.Number)
                .ToFrozenDictionary(g => g.Key, g => g.Last()) ?? FrozenDictionary<string, Models.Product>.Empty;

            if (products.Count > 0)
                memoryCache.Set(ProductsCacheKey, products, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ProductsCacheDuration,
                    Size = 1
                });
            else
                logger.LogWarning("Products SOAP response produced an empty product dictionary; result will not be cached.");

            return products;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while loading products from Polcar");
            return FrozenDictionary<string, Models.Product>.Empty;
        }
        finally
        {
            CacheLock.Release();
        }
    }
}
