using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Dtos;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public interface IDistributorsSalesClient
{
    /// <summary>
    /// Gets orders for the specified date.
    /// </summary>
    /// <param name="date">The date to retrieve orders for.</param>
    /// <returns>A collection of orders.</returns>
    Task<IEnumerable<DistributorOrderDto>> GetOrdersAsync(DateTime date);
}
