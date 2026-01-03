using Application.Common.Consts;
using Application.Common.Interfaces;
using Application.Common.Options;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolcarProductsClient;

namespace Infrastructure.Services.Polcar;

public class ProductsService(
    ILogger<ProductsService> logger,
    IOptions<PolcarOptions> polcarOptions,
    IMemoryCache cache) : IProductsService
{
    private readonly ProductsSoapClient _soapClient = new(ProductsSoapClient.EndpointConfiguration.ProductsSoap12);

    public async Task<IDictionary<string, Domain.Entities.Product>> GetProductsAsync()
    {
        if (cache.TryGetValue(CacheConsts.PolcarProducts, out IDictionary<string, Domain.Entities.Product> cachedProducts))
            return cachedProducts;

        var productsDictionary = new Dictionary<string, Domain.Entities.Product>();

        try
        {
            var response = await _soapClient.GetEAN13ListAsync(
                Login: polcarOptions.Value.Login,
                Password: polcarOptions.Value.Password,
                LanguageID: 1,
                FormatID: 1);

            var deserialized = response.Body.GetEAN13ListResult.Deserialize<ProductRoot>();

            var products = deserialized.Items.Select(product => new Domain.Entities.Product
            {
                EAN13Code = product.EAN13Code,
                Name = product.PartName,
                Number = product.Number
            });

            foreach (var product in products)
                productsDictionary[product.Number] = product;

            cache.Set(CacheConsts.PolcarProducts, productsDictionary, TimeSpan.FromHours(24));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve products from Polcar.");
        }

        return productsDictionary;
    }
}
