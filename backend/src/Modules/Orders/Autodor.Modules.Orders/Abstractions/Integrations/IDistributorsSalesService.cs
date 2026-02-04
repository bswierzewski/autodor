using Autodor.Modules.Orders.Domain.Models;

namespace Autodor.Modules.Orders.Abstractions.Integrations;

/// <summary>
/// Service for distributors sales external integration
/// </summary>
public interface IDistributorsSalesService
{
    /// <summary>
    /// Gets orders for the specified date.
    /// </summary>
    /// <param name="date">The date to retrieve orders for.</param>
    /// <returns>A collection of orders.</returns>
    Task<IEnumerable<Order>> GetOrdersAsync(DateTime date);
}
