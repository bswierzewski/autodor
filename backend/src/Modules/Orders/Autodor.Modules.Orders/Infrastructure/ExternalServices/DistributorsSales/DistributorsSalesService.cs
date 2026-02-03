using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesService(IOptions<DistributorsSalesOptions> options) : IDistributorsSalesService
{
    private readonly DistributorsSalesOptions _options = options.Value;
}
