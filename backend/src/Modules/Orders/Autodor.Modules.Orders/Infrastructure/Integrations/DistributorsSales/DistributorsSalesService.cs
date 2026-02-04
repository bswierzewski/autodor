using Autodor.Modules.Orders.Abstractions.Integrations;
using Autodor.Modules.Orders.Domain.Models;
using Autodor.Modules.Orders.Infrastructure.Consts;
using Autodor.Modules.Orders.Infrastructure.Extensions;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Extensions;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Factories;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesService(
        IOptions<DistributorsSalesOptions> options,
        IDistributorsSalesServiceClientFactory clientFactory,
        [FromKeyedServices(KeyedServicesConsts.DistributorsSalesSoap)] ResiliencePipeline resiliencePipeline
    ) : IDistributorsSalesService
{
    private readonly DistributorsSalesOptions _options = options.Value;

    public async Task<IEnumerable<Order>> GetOrdersAsync(DateTime date)
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
                throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");

            return responseBody.ListOfOrders?.ToDto() ?? [];
        }
        finally
        {
            await client.CloseClientAsync();
        }
    }
}
