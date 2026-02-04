using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public class DistributorsSalesService(IOptions<DistributorsSalesOptions> options) : Abstractions.Integrations.DistributorsSales.IDistributorsSalesService
{
    private readonly DistributorsSalesOptions _options = options.Value;
}
