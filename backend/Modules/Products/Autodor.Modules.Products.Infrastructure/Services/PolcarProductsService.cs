using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Domain.ValueObjects;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;
using Autodor.Modules.Products.Infrastructure.Helpers;

namespace Autodor.Modules.Products.Infrastructure.Services;

public class PolcarProductsService : IPolcarProductsService
{
    private readonly PolcarProductsOptions _options;
    private readonly ProductsSoapClient _soapClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PolcarProductsService> _logger;
    private const string CACHE_KEY_PRODUCTS = "PolcarProducts";

    public PolcarProductsService(
        IOptions<PolcarProductsOptions> options, 
        ProductsSoapClient soapClient,
        IMemoryCache cache,
        ILogger<PolcarProductsService> logger)
    {
        _options = options.Value;
        _soapClient = soapClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product> GetProductAsync(string partNumber)
    {
        var allProducts = await GetAllProductsAsync();
        return allProducts.TryGetValue(partNumber, out var product) 
            ? product 
            : throw new InvalidOperationException($"Product with part number '{partNumber}' not found");
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(IEnumerable<string> partNumbers)
    {
        var allProducts = await GetAllProductsAsync();
        return partNumbers
            .Where(partNumber => allProducts.ContainsKey(partNumber))
            .Select(partNumber => allProducts[partNumber])
            .ToList();
    }

    private async Task<IDictionary<string, Product>> GetAllProductsAsync()
    {
        if (_cache.TryGetValue(CACHE_KEY_PRODUCTS, out IDictionary<string, Product> cachedProducts))
            return cachedProducts;

        var productsDictionary = new Dictionary<string, Product>();

        try
        {
            var response = await _soapClient.GetEAN13ListAsync(
                Login: _options.Login,
                Password: _options.Password,
                LanguageID: _options.LanguageId,
                FormatID: 1);

            var deserialized = response.Body.GetEAN13ListResult.Deserialize<ProductRoot>();

            var products = deserialized.Items.Select(item => new Product(
                Name: item.PartName,
                PartNumber: item.Number
            ));

            foreach (var product in products)
                productsDictionary[product.PartNumber] = product;

            _cache.Set(CACHE_KEY_PRODUCTS, productsDictionary, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve products from Polcar.");
        }

        return productsDictionary;
    }
}