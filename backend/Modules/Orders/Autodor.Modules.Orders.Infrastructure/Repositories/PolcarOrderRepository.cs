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
/// Repository implementation for retrieving orders from the Polcar external system via SOAP API.
/// Includes retry policies for resilient communication with the external service.
/// </summary>
public class PolcarOrderRepository : IOrdersRepository
{
    private readonly PolcarSalesOptions _options;
    private readonly ILogger<PolcarOrderRepository> _logger;
    private readonly DistributorsSalesServiceSoapClient _soapClient;

    private readonly AsyncRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the PolcarOrderRepository with necessary dependencies and retry policy.
    /// </summary>
    /// <param name="options">Configuration options for Polcar API connection.</param>
    /// <param name="logger">Logger for tracking repository operations and errors.</param>
    /// <param name="soapClient">The SOAP client for communicating with Polcar service.</param>
    public PolcarOrderRepository(
        IOptions<PolcarSalesOptions> options,
        ILogger<PolcarOrderRepository> logger,
        DistributorsSalesServiceSoapClient soapClient)
    {
        _options = options.Value;
        _logger = logger;
        _soapClient = soapClient;

        // Configure retry policy for resilient external service calls
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5),
                onRetry: (outcome, duration, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount}/3 for SOAP call after {Duration}s delay", retryCount, duration.TotalSeconds);
                });
    }

    /// <summary>
    /// Retrieves all orders for a specific date from the Polcar system.
    /// </summary>
    /// <param name="date">The date for which to retrieve orders.</param>
    /// <returns>A collection of orders for the specified date.</returns>
    public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
    {
        return await GetOrdersByDateRangeAsync(date.Date, date.Date);
    }

    /// <summary>
    /// Retrieves all orders within a specified date range from the Polcar system using parallel processing for each day.
    /// </summary>
    /// <param name="dateFrom">The start date of the range (inclusive).</param>
    /// <param name="dateTo">The end date of the range (inclusive).</param>
    /// <returns>A collection of orders within the specified date range.</returns>
    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            _logger.LogInformation("Fetching orders from {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);

            var dates = DateTimeExtensions.EachDay(dateFrom, dateTo).ToList();

            _logger.LogInformation("Split date range into {DateCount} days for parallel processing", dates.Count);

            var tasks = dates.Select(FetchOrdersForSingleDateAsync);
            var results = await Task.WhenAll(tasks);

            var allOrders = results.SelectMany(orders => orders);

            _logger.LogInformation("Successfully retrieved orders from all dates in range {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);

            return allOrders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching orders from {DateFrom} to {DateTo}", dateFrom.Date, dateTo.Date);
            throw;
        }
    }

    /// <summary>
    /// Fetches orders for a single date from the Polcar SOAP service with retry policy.
    /// </summary>
    /// <param name="date">The specific date to fetch orders for.</param>
    /// <returns>A collection of orders for the specified date.</returns>
    private async Task<IEnumerable<Order>> FetchOrdersForSingleDateAsync(DateTime date)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Fetching orders for date {Date}", date.Date);

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

            return responseBody?.ListOfOrders?.Length > 0
                ? responseBody.ListOfOrders.Select(MapToOrder)
                : [];
        });
    }

    /// <summary>
    /// Maps a Polcar SOAP response to a domain Order entity.
    /// </summary>
    /// <param name="response">The SOAP response containing order data.</param>
    /// <returns>A mapped Order entity.</returns>
    private static Order MapToOrder(DistributorSalesOrderResponse response)
    {
        return new Order
        {
            EntryDate = response.EntryDate,
            Id = response.OrderID,
            Number = response.PolcarOrderNumber,

            Contractor = new OrderContractor
            {
                Name = response.OrderingPerson,
                Number = response.CustomerNumber,
            },

            Items = response.OrderedItemsResponse?.Select(MapToOrderItem).ToList() ?? []
        };
    }

    /// <summary>
    /// Maps a Polcar SOAP response item to a domain OrderItem entity.
    /// </summary>
    /// <param name="response">The SOAP response containing order item data.</param>
    /// <returns>A mapped OrderItem entity.</returns>
    private static OrderItem MapToOrderItem(DistributorSalesOrderItemResponse response)
    {
        return new OrderItem
        {
            OrderId = response.OrderId,
            Number = response.PartNumber,

            Quantity = response.QuantityOrdered,
            Price = response.Price
        };
    }


}