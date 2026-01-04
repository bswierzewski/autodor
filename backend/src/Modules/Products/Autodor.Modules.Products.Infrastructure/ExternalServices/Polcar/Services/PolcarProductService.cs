using Autodor.Modules.Products.Application.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Abstractions.Extensions;
using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;

/// <summary>
/// Implementation of the Polcar product service that retrieves product data via SOAP API with retry logic.
/// </summary>
public class PolcarProductService : IPolcarProductService
{
    private readonly PolcarProductsOptions _options;
    private readonly ProductsSoapClient _soapClient;
    private readonly ILogger<PolcarProductService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the PolcarProductService with SOAP client and retry policy configuration.
    /// </summary>
    /// <param name="options">Configuration options for Polcar authentication</param>
    /// <param name="soapClient">SOAP client for Polcar API communication</param>
    /// <param name="logger">Logger for service operations</param>
    public PolcarProductService(
        IOptions<PolcarProductsOptions> options,
        ProductsSoapClient soapClient,
        ILogger<PolcarProductService> logger)
    {
        _options = options.Value;
        _soapClient = soapClient;
        _logger = logger;

        // Configure exponential backoff retry policy for resilience
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

    /// <summary>
    /// Retrieves all products from the Polcar SOAP API with automatic retry on failures.
    /// Deserializes XML response and maps to domain Product entities.
    /// </summary>
    /// <returns>Collection of products from Polcar, or empty collection on failure</returns>
    public async Task<IEnumerable<Domain.Entities.Product>> GetProductsAsync()
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

                var deserialized = response.Body.GetEAN13ListResult.OuterXml.FromXml<ProductRoot>();

                var products = deserialized.Items.Select(item => new Domain.Entities.Product
                {
                    Name = item.PartName,
                    Number = item.Number,
                    EAN13 = item.EAN13Code
                }).ToList();

                _logger.LogInformation("Successfully loaded {Count} products from Polcar", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading products from Polcar");

                return Enumerable.Empty<Domain.Entities.Product>();
            }
        });
    }
}