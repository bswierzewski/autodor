using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;
using BuildingBlocks.Extensions;

namespace Autodor.Modules.Orders.Infrastructure.Repositories;

/// <summary>
/// Repository implementation that retrieves order data from the Polcar external system.
/// This class provides resilient communication with the Polcar SOAP service,
/// including retry policies and error handling for external service integration.
/// Maps external service responses to internal domain entities.
/// </summary>
public class PolcarOrderRepository : IOrdersRepository
{
    private readonly PolcarSalesOptions _options;
    private readonly ILogger<PolcarOrderRepository> _logger;
    private readonly DistributorsSalesServiceSoapClient _soapClient;

    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the PolcarOrderRepository.
    /// Sets up external service integration with resilience policies for reliable data retrieval.
    /// </summary>
    /// <param name="options">Configuration options for Polcar service connection</param>
    /// <param name="logger">Logger for tracking external service operations and errors</param>
    /// <param name="soapClient">SOAP client for communicating with Polcar service</param>
    public PolcarOrderRepository(
        IOptions<PolcarSalesOptions> options,
        ILogger<PolcarOrderRepository> logger,
        DistributorsSalesServiceSoapClient soapClient)
    {
        _options = options.Value;
        _logger = logger;
        _soapClient = soapClient;

        // Configure retry policy for resilient external service communication
        // This handles transient failures that are common with external SOAP services
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5),
                onRetry: (outcome, duration, retryCount, context) =>
                {
                    // Log retry attempts for monitoring and debugging external service issues
                    _logger.LogWarning("Retry {RetryCount}/3 for SOAP call after {Duration}s delay", retryCount, duration.TotalSeconds);
                });
    }

    /// <summary>
    /// Retrieves all orders for a specific date from the Polcar external system.
    /// This method delegates to the date range method for consistent processing logic.
    /// </summary>
    /// <param name="date">The specific date to retrieve orders for</param>
    /// <returns>A collection of orders for the specified date</returns>
    public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
    {
        // Delegate to date range method with same start and end date for consistent processing
        // This ensures all date-based queries follow the same retry and mapping logic
        return await GetOrdersByDateRangeAsync(date.Date, date.Date);
    }

    /// <summary>
    /// Retrieves all orders within a specified date range from the Polcar external system.
    /// Uses parallel processing to improve performance when fetching data across multiple dates.
    /// </summary>
    /// <param name="dateFrom">The start date of the range (inclusive)</param>
    /// <param name="dateTo">The end date of the range (inclusive)</param>
    /// <returns>A collection of orders within the specified date range</returns>
    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            // Log the start of the date range operation for monitoring and debugging
            _logger.LogInformation("Fetching orders from {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);

            // Generate list of individual dates within the range for parallel processing
            // This approach optimizes external service calls by processing dates concurrently
            var dates = DateTimeExtensions.EachDay(dateFrom, dateTo).ToList();

            _logger.LogInformation("Split date range into {DateCount} days for parallel processing", dates.Count);

            // Execute SOAP calls in parallel to improve performance
            // Each date is processed independently to maximize throughput
            var tasks = dates.Select(FetchOrdersForSingleDateAsync);
            var results = await Task.WhenAll(tasks);

            // Flatten results from all date queries into a single collection
            // This provides a unified view of orders across the entire date range
            var allOrders = results.SelectMany(orders => orders);

            _logger.LogInformation("Successfully retrieved orders from all dates in range {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);

            return allOrders;
        }
        catch (Exception ex)
        {
            // Log and re-throw exceptions to maintain error transparency
            // This ensures calling code can handle external service failures appropriately
            _logger.LogError(ex, "Error occurred while fetching orders from {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);
            throw;
        }
    }

    /// <summary>
    /// Fetches orders for a single date from the Polcar SOAP service with retry policy.
    /// This method handles the actual external service call and response mapping.
    /// </summary>
    /// <param name="date">The specific date to fetch orders for</param>
    /// <returns>A collection of orders for the specified date</returns>
    private async Task<IEnumerable<Order>> FetchOrdersForSingleDateAsync(DateTime date)
    {
        // Execute the SOAP call within retry policy to handle transient failures
        // This ensures resilient communication with the external Polcar service
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Fetching orders for date {Date}", date.Date);

            // Call Polcar SOAP service with configured credentials and parameters
            // This retrieves raw order data from the external system
            var response = await _soapClient.GetListOfOrdersV3Async(
                distributorCode: _options.DistributorCode,
                getOpenOrdersOnly: false,
                branchId: _options.BranchId,
                dateFrom: date.Date,
                dateTo: date.Date.AddDays(1),
                getOrdersHeadersOnly: false,
                login: _options.Login,
                password: _options.Password,
                languageId: _options.LanguageId
            );

            var responseBody = response.Body.GetListOfOrdersV3Result;

            _logger.LogInformation("Successfully retrieved orders for date {Date}", date.Date);

            // Map external service response to internal domain entities
            // Return empty collection if no orders found to maintain consistent behavior
            return responseBody?.ListOfOrders?.Length > 0
                ? responseBody.ListOfOrders.Select(MapToOrder)
                : [];
        });
    }

    /// <summary>
    /// Maps external Polcar order response to internal domain Order entity.
    /// This method performs the translation between external service data and internal domain model.
    /// </summary>
    /// <param name="response">The Polcar SOAP service order response</param>
    /// <returns>A domain Order entity with mapped data</returns>
    private static Order MapToOrder(DistributorSalesOrderResponse response)
    {
        return new Order
        {
            // Map basic order information from external service to domain entity
            EntryDate = response.EntryDate,
            Id = response.OrderID,
            Number = response.PolcarOrderNumber,

            // Map contractor information to embedded value object
            // This encapsulates customer details within the order context
            Contractor = new OrderContractor
            {
                Name = response.OrderingPerson,
                Number = response.CustomerNumber,
            },

            // Map order items collection, handling null responses gracefully
            // This ensures the domain entity always has a valid items collection
            Items = response.OrderedItemsResponse?.Select(MapToOrderItem).ToList() ?? []
        };
    }

    /// <summary>
    /// Maps external Polcar order item response to internal domain OrderItem entity.
    /// This method handles the translation of individual line items from external to internal format.
    /// </summary>
    /// <param name="response">The Polcar SOAP service order item response</param>
    /// <returns>A domain OrderItem entity with mapped data</returns>
    private static OrderItem MapToOrderItem(DistributorSalesOrderItemResponse response)
    {
        return new OrderItem
        {
            // Map item relationship and identification data
            OrderId = response.OrderId,
            Number = response.PartNumber,

            // Map quantity and pricing information for business calculations
            Quantity = response.QuantityOrdered,
            Price = response.Price
        };
    }


}