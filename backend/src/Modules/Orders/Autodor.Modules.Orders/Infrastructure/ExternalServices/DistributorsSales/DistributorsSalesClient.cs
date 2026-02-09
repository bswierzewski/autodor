using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Dtos;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Extensions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.ServiceReference;
using BuildingBlocks.Infrastructure.Soap.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesClient(
        IOptions<DistributorsSalesOptions> options,
        ISoapInvoker<DistributorsSalesServiceClient> soapInvoker,
        ILogger<DistributorsSalesClient> logger
    ) : IDistributorsSalesClient
{
    private readonly DistributorsSalesOptions _options = options.Value;

    public async Task<IEnumerable<DistributorOrderDto>> GetOrdersAsync(DateTime date)
    {
        var response = await soapInvoker.InvokeAsync(async client =>
        {
            return await client.GetListOfOrdersV3Async(
                distributorCode: _options.DistributorCode,
                getOpenOrdersOnly: false,
                branchId: _options.BranchId,
                dateFrom: date.Date,
                dateTo: date.AddDays(1).Date,
                getOrdersHeadersOnly: false,
                login: _options.Login,
                password: _options.Password,
                languageId: _options.LanguageId
            );
        });

        var responseBody = response.Body.GetListOfOrdersV3Result;

        if (responseBody.ErrorCode != "0")
        {
            logger.LogError("DistributorsSales API returned error. ErrorCode: {ErrorCode}, ErrorInformation: {ErrorInformation}",
                responseBody.ErrorCode, responseBody.ErrorInformation);

            throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");
        }

        var orders = responseBody.ListOfOrders?.ToDto() ?? [];

        logger.LogInformation("Successfully fetched {Count} orders from DistributorsSales API for date {Date}", orders.Count(), date.Date);

        return orders;
    }
}
