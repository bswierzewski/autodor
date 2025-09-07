using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;

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

            var dates = Enumerable.Range(0, (dateFrom - dateTo).Days + 1)
                     .Select(offset => dateTo.AddDays(offset))
                     .ToList();

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
                dateTo: date.Date,
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

    private static Order MapToOrder(DistributorSalesOrderResponse response)
    {
        return new Order
        {
            Date = response.EntryDate,
            Id = response.OrderID,
            Number = response.PolcarOrderNumber,
            Contractor = response.CustomerNumber,
            Items = response.OrderedItemsResponse?.Select(MapToOrderItem) ?? Enumerable.Empty<OrderItem>()
        };
    }

    private static OrderItem MapToOrderItem(DistributorSalesOrderItemResponse response)
    {
        return new OrderItem
        {
            Number = response.PartNumber,
            Name = response.PartNumber,
            Quantity = response.QuantityOrdered,
            Price = response.Price
        };
    }

}