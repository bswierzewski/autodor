using Autodor.Modules.Orders.Abstractions.Integrations.DistributorsSales;
using Autodor.Modules.Orders.Abstractions.Integrations.DistributorsSales.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesService(IOptions<DistributorsSalesOptions> options) : IDistributorsSalesService
{
    private readonly DistributorsSalesOptions _options = options.Value;

    public Task<IEnumerable<DistributorsSalesOrderDto>> GetOrdersAsync(DateTime date)
    {
        throw new NotImplementedException();
    }
}
