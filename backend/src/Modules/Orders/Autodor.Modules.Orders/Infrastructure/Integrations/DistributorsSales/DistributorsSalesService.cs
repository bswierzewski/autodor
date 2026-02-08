using Autodor.Modules.Orders.Infrastructure.Consts;
using Autodor.Modules.Orders.Infrastructure.Extensions;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Dtos;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Extensions;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Factories;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesService(
        IOptions<DistributorsSalesOptions> options,
        IDistributorsSalesServiceClientFactory clientFactory,
        [FromKeyedServices(KeyedServicesConsts.DistributorsSalesSoap)] ResiliencePipeline resiliencePipeline,
        ILogger<DistributorsSalesService> logger
    ) : IDistributorsSalesService
{
    private readonly DistributorsSalesOptions _options = options.Value;

    public async Task<IEnumerable<DistributorOrderDto>> GetOrdersAsync(DateTime date)
    {
        var client = clientFactory.Create();

        try
        {
            var response = await resiliencePipeline.ExecuteAsync(async ct =>
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while loading orders from DistributorsSales API for date {Date}", date.Date);
            throw;
        }
        finally
        {
            await client.CloseClientAsync();
        }
    }
}
