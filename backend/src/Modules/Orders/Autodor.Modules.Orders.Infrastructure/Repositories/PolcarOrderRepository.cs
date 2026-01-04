using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Application.Options;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using BuildingBlocks.Abstractions.Extensions;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Autodor.Modules.Orders.Infrastructure.Repositories;

public class PolcarOrderRepository : IOrdersRepository
{
    private readonly PolcarSalesOptions _options;
    private readonly ILogger<PolcarOrderRepository> _logger;
    private readonly DistributorsSalesServiceSoapClient _soapClient;

    private readonly AsyncRetryPolicy _retryPolicy;

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

    public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
    {
        return await GetOrdersByDateRangeAsync(date.Date, date.Date);
    }

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

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Fetching order {OrderId}", orderId);

            var response = await _soapClient.GetOrderDetailsAsync(
                DistributorCode: _options.DistributorCode,
                OrderID: orderId,
                Login: _options.Login,
                Password: _options.Password,
                LanguageID: _options.LanguageId
            );

            var soapOrder = response.Body.GetOrderDetailsResult;

            if (soapOrder == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return null;
            }

            var order = soapOrder.MapToOrder();
            _logger.LogInformation("Successfully found order {OrderId}", orderId);

            return order;
        });
    }

    public async Task<Order?> GetOrderByIdAndDateAsync(string orderId, DateTime date)
    {
        _logger.LogInformation("Fetching order {OrderId} for date {Date}", orderId, date.Date);

        var orders = await GetOrdersByDateAsync(date);
        var order = orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for date {Date}", orderId, date.Date);
        }
        else
        {
            _logger.LogInformation("Successfully found order {OrderId} for date {Date}", orderId, date.Date);
        }

        return order;
    }

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
                ? responseBody.ListOfOrders.Select(order => order.MapToOrder())
                : [];
        });
    }
}