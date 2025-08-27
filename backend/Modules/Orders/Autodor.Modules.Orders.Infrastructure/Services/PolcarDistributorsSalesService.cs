using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using Microsoft.Extensions.Logging;
using PolcarDistributorsSalesService;

namespace Autodor.Modules.Orders.Infrastructure.Services;

public class PolcarDistributorsSalesService(ILogger<PolcarDistributorsSalesService> _logger) : IPolcarDistributorsSalesService
{
    private readonly DistributorsSalesServiceSoapClient _client = new(endpointConfiguration: DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap12);

    public Task<IEnumerable<Order>> GetOrdersAsync(DateTime dateFrom, DateTime dateTo, Guid contractorId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<string> orderNumbers)
    {
        throw new NotImplementedException();
    }
}