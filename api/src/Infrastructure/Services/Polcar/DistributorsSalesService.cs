using Application.Common.Interfaces;
using Application.Common.Options;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolcarDistributorsSalesClient;
using Polly;
using Polly.Retry;

namespace Infrastructure.Services.Polcar;

public class DistributorsSalesService(IMapper mapper, IOptions<PolcarOptions> polcarOptions, ILogger<DistributorsSalesService> logger) : IDistributorsSalesService
{
    private readonly DistributorsSalesServiceSoapClient _client = new(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap12);
    private readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3, // Number of retry attempts
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Log retry information
                    logger.LogError(exception: exception, message: $"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                });

    public async Task<IEnumerable<Order>> GetOrdersAsync(DateTime date)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.GetListOfOrdersV3Async(
                distributorCode: polcarOptions.Value.DistributorCode,
                getOpenOrdersOnly: false,
                branchId: polcarOptions.Value.BranchId,
                dateFrom: date.Date,
                dateTo: date.AddDays(1).Date,
                getOrdersHeadersOnly: false,
                login: polcarOptions.Value.Login,
                password: polcarOptions.Value.Password,
                languageId: polcarOptions.Value.LanguageId
            );

            var responseBody = response.Body.GetListOfOrdersV3Result;

            if (responseBody.ErrorCode != "0")
                throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");

            if (responseBody.ListOfOrders?.Count > 0)
                return mapper.Map<IEnumerable<Order>>(responseBody.ListOfOrders);

            return new List<Order>();
        });
    }
}