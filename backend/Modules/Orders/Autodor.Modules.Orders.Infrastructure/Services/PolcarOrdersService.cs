using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Polly;
using Polly.Retry;
using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;

namespace Autodor.Modules.Orders.Infrastructure.Services;

public class PolcarOrdersService : IPolcarOrdersService
{
    private readonly PolcarSalesOptions _options;
    private readonly IMapper _mapper;
    private readonly ILogger<PolcarOrdersService> _logger;
    private readonly DistributorsSalesServiceSoapClient _soapClient;

    private readonly AsyncRetryPolicy _retryPolicy;

    public PolcarOrdersService(
        IOptions<PolcarSalesOptions> options,
        IMapper mapper,
        ILogger<PolcarOrdersService> logger,
        DistributorsSalesServiceSoapClient soapClient)
    {
        _options = options.Value;
        _mapper = mapper;
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

            if (responseBody.ListOfOrders?.Count > 0)
                return _mapper.Map<IEnumerable<Order>>(responseBody.ListOfOrders);

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
                return _mapper.Map<Order>(responseBody);

            return new Order();
        });
    }
}