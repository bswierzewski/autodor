using Autodor.Modules.Products.Domain.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PolcarProductsService;

namespace Autodor.Modules.Products.Infrastructure.Services;

public class PolcarProductsService : IPolcarProductsService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<PolcarProductsService> _logger;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);
    private readonly ProductsSoapClient _productsSoapClient;

    public PolcarProductsService(IMemoryCache cache, ILogger<PolcarProductsService> logger)
    {
        _productsSoapClient = new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap12);
        _cache = cache;
        _logger = logger;
    }

    public Task<Domain.ValueObjects.Product> GetProductAsync(string partNumber)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Domain.ValueObjects.Product>> GetProductsAsync(IEnumerable<string> partNumbers)
    {
        throw new NotImplementedException();
    }
}