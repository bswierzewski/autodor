using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using BuildingBlocks.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;

/// <summary>
/// Service responsible for integrating with the Polcar SOAP API to retrieve product information.
/// Implements retry policies and error handling for reliable external service communication.
/// </summary>
public class PolcarProductService : IPolcarProductService
{
    private readonly PolcarProductsOptions _options;
    private readonly ProductsSoapClient _soapClient;
    private readonly ILogger<PolcarProductService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the PolcarProductService with dependency injection.
    /// Configures exponential backoff retry policy for handling transient failures.
    /// </summary>
    /// <param name="options">Configuration options for Polcar API integration including credentials and parameters</param>
    /// <param name="soapClient">SOAP client for communicating with Polcar web services</param>
    /// <param name="logger">Logger instance for tracking operations and errors</param>
    public PolcarProductService(
        IOptions<PolcarProductsOptions> options,
        ProductsSoapClient soapClient,
        ILogger<PolcarProductService> logger)
    {
        _options = options.Value;
        _soapClient = soapClient;
        _logger = logger;

        // Configure retry policy with exponential backoff to handle transient network issues
        // Business rationale: External API calls may fail due to network issues or temporary service unavailability
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
    /// Retrieves all products from the Polcar external service via SOAP API.
    /// This method fetches the complete product catalog including part numbers, names, and EAN codes.
    /// </summary>
    /// <returns>Collection of Product entities populated from external service data, or empty collection on failure</returns>
    public async Task<IEnumerable<Domain.Aggregates.Product>> GetProductsAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                // Call Polcar SOAP service to retrieve EAN13 product list
                // Using configured credentials and language/format preferences
                var response = await _soapClient.GetEAN13ListAsync(
                    Login: _options.Login,
                    Password: _options.Password,
                    LanguageID: _options.LanguageId,
                    FormatID: _options.FormatId);

                // Deserialize XML response to strongly-typed objects
                // The response contains nested XML that needs parsing
                var deserialized = response.Body.GetEAN13ListResult.OuterXml.FromXml<ProductRoot>();

                // Transform external API data model to internal domain entities
                // Business rule: Map Polcar fields to our standardized product structure
                var products = deserialized.Items.Select(item => new Domain.Aggregates.Product
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

                // Return empty collection to prevent cascade failures in synchronization
                // Business decision: Better to have no updates than corrupt data
                return Enumerable.Empty<Domain.Aggregates.Product>();
            }
        });
    }
}