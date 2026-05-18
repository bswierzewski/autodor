using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.GetOrders;

/// <summary>
/// Query for GetOrders endpoint
/// </summary>
public class GetOrdersQuery
{
    [FromQuery(Name = "from")]
    public DateTime From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime To { get; set; }
}
