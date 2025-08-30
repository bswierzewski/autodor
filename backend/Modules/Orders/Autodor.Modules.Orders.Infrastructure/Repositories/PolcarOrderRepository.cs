using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Options;

namespace Autodor.Modules.Orders.Infrastructure.Repositories;

public class PolcarOrderRepository : IOrderRepository
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
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(exception, "Retry {RetryCount} encountered an error: {Message}. Waiting {TimeSpan} before next retry.",
                        retryCount, exception.Message, timeSpan);
                });
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(DateTime dateFrom, DateTime dateTo, Guid contractorId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _soapClient.GetListOfOrdersV3Async(
                distributorCode: _options.DistributorCode,
                getOpenOrdersOnly: false,
                branchId: _options.BranchId,
                dateFrom: dateFrom.Date,
                dateTo: dateTo.Date,
                getOrdersHeadersOnly: false,
                login: _options.Login,
                password: _options.Password,
                languageId: _options.LanguageId
            );

            var responseBody = response.Body.GetListOfOrdersV3Result;

            if (responseBody.ErrorCode != "0")
                throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");

            if (responseBody.ListOfOrders?.Length > 0)
                return MapDistributorOrdersToOrders(responseBody.ListOfOrders!);

            return new List<Order>();
        });
    }

    public async Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<string> orderNumbers)
    {
        var orders = new List<Order>();

        foreach (var orderNumber in orderNumbers)
        {
            try
            {
                var order = await GetOrderByIdAsync(orderNumber);
                orders.Add(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve order {OrderNumber}", orderNumber);
            }
        }

        return orders;
    }

    private async Task<Order> GetOrderByIdAsync(string orderNumber)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _soapClient.GetOrderDetailsAsync(
                DistributorCode: _options.DistributorCode,
                OrderID: orderNumber,
                Login: _options.Login,
                Password: _options.Password,
                LanguageID: _options.LanguageId
            );

            var responseBody = response.Body.GetOrderDetailsResult;

            if (responseBody.ErrorCode != "0")
                throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");

            if (responseBody != null)
                return MapToOrder(responseBody);

            return new Order();
        });
    }

    private IEnumerable<Order> MapDistributorOrdersToOrders(DistributorSalesOrderResponse[] salesOrders)
    {
        return salesOrders.Select(so => MapToOrder(so));
    }

    private Order MapToOrder(SalesOrderResponse salesOrder)
    {
        return new Order
        {
            Id = salesOrder.OrderID ?? string.Empty,
            Number = salesOrder.PolcarOrderNumber ?? string.Empty,
            Date = salesOrder.ShipmentDate,
            Person = salesOrder.OrderingPerson ?? string.Empty,
            CustomerNumber = salesOrder.CustomerNumber ?? string.Empty,
            Items = MapToOrderItems(salesOrder.OrderedItemsResponse)
        };
    }

    private Order MapToOrder(DistributorSalesOrderResponse salesOrder)
    {
        return new Order
        {
            Id = salesOrder.OrderID ?? string.Empty,
            Number = salesOrder.PolcarOrderNumber ?? string.Empty,
            Date = salesOrder.ShipmentDate,
            Person = salesOrder.OrderingPerson ?? string.Empty,
            CustomerNumber = salesOrder.CustomerNumber ?? string.Empty,
            Items = MapToOrderItems(salesOrder.OrderedItemsResponse)
        };
    }

    private IEnumerable<OrderItem> MapToOrderItems(SalesOrderItemResponse[]? items)
    {
        if (items == null) return Enumerable.Empty<OrderItem>();

        return items.Select(item => new OrderItem
        {
            PartNumber = item.PolcarPartNumber ?? string.Empty,
            PartName = item.CustomerPartNumber ?? string.Empty,
            Quantity = item.QuantityOrdered,
            TotalPrice = item.CustomerPrice
        });
    }

    private IEnumerable<OrderItem> MapToOrderItems(DistributorSalesOrderItemResponse[]? items)
    {
        if (items == null) return Enumerable.Empty<OrderItem>();

        return items.Select(item => new OrderItem
        {
            PartNumber = item.PartNumber ?? string.Empty,
            PartName = item.PartNumber ?? string.Empty, // Using PartNumber as name since no separate name field exists
            Quantity = item.QuantityOrdered,
            TotalPrice = item.Price
        });
    }
}