using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Infrastructure.Helpers;
using Autodor.Modules.Products.Infrastructure.Models;
using Autodor.Modules.Products.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolcarProductsService;
using Polly;
using System.ServiceModel;

namespace Autodor.Modules.Products.Infrastructure.Services;

public class PolcarProductsService(
    IMemoryCache cache,
    ILogger<PolcarProductsService> logger,
    IOptions<PolcarOptions> polcarOptions) : IPolcarProductsService
{
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(24);
    private readonly ProductsSoapClient _productsSoapClient = new(ProductsSoapClient.EndpointConfiguration.ProductsSoap12);

    private readonly IAsyncPolicy _retryPolicy = Policy
        .Handle<CommunicationException>()
        .Or<TimeoutException>()
        .Or<EndpointNotFoundException>()
        .Or<ServerTooBusyException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(10),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                logger.LogWarning("Polcar API call failed. Retry attempt {RetryCount} in {Delay}ms. Exception: {Exception}",
                    retryCount, timespan.TotalMilliseconds, outcome.Message);
            });

    private const string CacheKey = "PolcarProducts";

    public async Task<Domain.ValueObjects.Product> GetProductAsync(string partNumber)
    {
        var products = await GetAllProductsAsync();
        return products.FirstOrDefault(p => p.PartNumber == partNumber);
    }

    public async Task<IEnumerable<Domain.ValueObjects.Product>> GetProductsAsync(IEnumerable<string> partNumbers)
    {
        var allProducts = await GetAllProductsAsync();
        var partNumbersSet = partNumbers.ToHashSet();
        return allProducts.Where(p => partNumbersSet.Contains(p.PartNumber));
    }

    private async Task<IEnumerable<Domain.ValueObjects.Product>> GetAllProductsAsync()
    {
        if (cache.TryGetValue(CacheKey, out IEnumerable<Domain.ValueObjects.Product> cachedProducts))
            return cachedProducts;

        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _productsSoapClient.GetEAN13ListAsync(
                    Login: polcarOptions.Value.Login,
                    Password: polcarOptions.Value.Password,
                    LanguageID: 1,
                    FormatID: 1));

            var deserialized = response.Body.GetEAN13ListResult.OuterXml.DeserializeXml<ProductRoot>();

            var products = deserialized.Items.Select(item => new Domain.ValueObjects.Product(
                Name: item.PartName,
                PartNumber: item.Number
            ));

            cache.Set(CacheKey, products, _cacheExpiry);
            return products;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve products from Polcar after {RetryCount} retries",
                polcarOptions.Value.RetryPolicy.RetryCount);
            return Enumerable.Empty<Domain.ValueObjects.Product>();
        }
    }

}