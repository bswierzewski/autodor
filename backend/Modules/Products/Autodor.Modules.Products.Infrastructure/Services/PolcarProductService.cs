using Autodor.Modules.Products.Infrastructure.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;
using Autodor.Modules.Products.Infrastructure.Helpers;
using Autodor.Modules.Products.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Autodor.Modules.Products.Infrastructure.Services;

public class PolcarProductService : IPolcarProductService
{
    private readonly PolcarProductsOptions _options;
    private readonly ProductsSoapClient _soapClient;
    private readonly ILogger<PolcarProductService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public PolcarProductService(
        IOptions<PolcarProductsOptions> options,
        ProductsSoapClient soapClient,
        ILogger<PolcarProductService> logger)
    {
        _options = options.Value;
        _soapClient = soapClient;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(exception, "Retry {RetryCount} encountered an error: {Message}. Waiting {TimeSpan} before next retry.",
                        retryCount, exception.Message, timeSpan);
                });
    }

    public async Task<IEnumerable<Domain.ValueObjects.Product>> GetProductsAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var response = await _soapClient.GetEAN13ListAsync(
                    Login: _options.Login,
                    Password: _options.Password,
                    LanguageID: _options.LanguageId,
                    FormatID: _options.FormatId);
                
                var deserialized = response.Body.GetEAN13ListResult.OuterXml.DeserializeXml<ProductRoot>();

                var products = deserialized.Items.Select(item => new Domain.ValueObjects.Product(
                    Name: item.PartName,
                    PartNumber: item.Number,
                    Ean: item.EAN13Code
                )).ToList();

                _logger.LogInformation("Załadowano {Count} produktów z Polcar", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ładowania produktów z Polcar");

                return Enumerable.Empty<Domain.ValueObjects.Product>();
            }
        });
    }
}