using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.ServiceReference;
using BuildingBlocks.Soap;
using BuildingBlocks.Soap.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesClient(
        IOptions<DistributorsSalesOptions> options,
        SoapPipeline<DistributorsSalesServiceClient> soapPipeline,
        ILogger<DistributorsSalesClient> logger
    ) : IDistributorsSalesClient
{
    private readonly DistributorsSalesOptions _options = options.Value;

    public async Task<IEnumerable<DistributorOrder>> GetOrdersAsync(DateTime date)
    {
        var response = await soapPipeline.InvokeAsync(
            async client =>
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
            },
            SoapCallContext.ForCache(
                nameof(GetOrdersAsync),
                DateOnly.FromDateTime(date)));

        var responseBody = response.Body.GetListOfOrdersV3Result;

        if (responseBody.ErrorCode != "0")
        {
            // The SOAP call itself succeeded, but the partner API reported a business error in the payload.
            logger.LogError("DistributorsSales API returned error. ErrorCode: {ErrorCode}, ErrorInformation: {ErrorInformation}",
                responseBody.ErrorCode, responseBody.ErrorInformation);

            throw new Exception($"{responseBody.ErrorCode} - {responseBody.ErrorInformation}");
        }

        // Map from SOAP response to application model
        var orders = responseBody.ListOfOrders?
            .Where(soap => soap is not null)
            .Select(soap => new DistributorOrder
            {
                Id = soap.OrderID ?? string.Empty,
                Number = soap.PolcarOrderNumber ?? string.Empty,
                Date = soap.EntryDate,
                Person = soap.OrderingPerson ?? string.Empty,
                CustomerNumber = soap.CustomerNumber ?? string.Empty,
                Items = soap.OrderedItemsResponse?
                    .Where(item => item is not null)
                    .Select(item => new DistributorOrderItem
                    {
                        PartNumber = item.PartNumber ?? string.Empty,
                        Quantity = item.QuantityOrdered,
                        Price = item.Price
                    })
                    .ToList() ?? []
            })
            .ToList() ?? [];

        logger.LogInformation("Successfully fetched {Count} orders from DistributorsSales API for date {Date}", orders.Count, date.Date);

        return orders;
    }
}
