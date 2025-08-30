using System.Collections.Concurrent;
using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Products.Infrastructure.Repositories;

public class PolcarProductRepository : IProductRepository
{
    private readonly PolcarProductsOptions _options;
    private readonly ProductsSoapClient _soapClient;
    private readonly ILogger<PolcarProductRepository> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "products:polcar:all";

    public PolcarProductRepository(
        IOptions<PolcarProductsOptions> options,
        ProductsSoapClient soapClient,
        ILogger<PolcarProductRepository> logger,
        IMemoryCache cache)
    {
        _options = options.Value;
        _soapClient = soapClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Domain.ValueObjects.Product> GetProductAsync(string partNumber)
    {
        var productsDictionary = await GetProductDictionaryAsync();

        if (productsDictionary.TryGetValue(partNumber, out var product))
        {
            return product;
        }

        throw new InvalidOperationException($"Product with part number '{partNumber}' not found");
    }

    public async Task<IEnumerable<Domain.ValueObjects.Product>> GetProductsAsync(IEnumerable<string> partNumbers)
    {
        var allProducts = await GetProductDictionaryAsync();

        var result = new List<Domain.ValueObjects.Product>();
        foreach (var partNumber in partNumbers)
        {
            if (allProducts.TryGetValue(partNumber, out var product))
            {
                result.Add(product);
            }
        }
        return result;
    }

    private async Task<ConcurrentDictionary<string, Domain.ValueObjects.Product>> GetProductDictionaryAsync()
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            _logger.LogInformation("Ładowanie produktów z Polcar i tworzenie słownika dla cache");

            var productList = await LoadProductsFromExternalServiceAsync();

            var productDictionary = new ConcurrentDictionary<string, Domain.ValueObjects.Product>(
                productList.ToDictionary(p => p.PartNumber, p => p)
            );

            _logger.LogInformation("Stworzono słownik z {Count} produktami dla cache.", productDictionary.Count);
            return productDictionary;
        }) ?? new ConcurrentDictionary<string, Domain.ValueObjects.Product>();
    }

    private async Task<IEnumerable<Domain.ValueObjects.Product>> LoadProductsFromExternalServiceAsync()
    {
        try
        {
            var response = await _soapClient.GetEAN13ListAsync(
                Login: _options.Login,
                Password: _options.Password,
                LanguageID: _options.LanguageId,
                FormatID: 1);

            var deserialized = response.Body.GetEAN13ListResult.InnerXml.DeserializeXml<ProductRoot>();

            var products = deserialized.Items.Select(item => new Domain.ValueObjects.Product(
                Name: item.PartName,
                PartNumber: item.Number
            )).ToList();

            _logger.LogInformation("Załadowano {Count} produktów z Polcar", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas ładowania produktów z Polcar");
            throw;
        }
    }
}