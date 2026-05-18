using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.GetOrder;

/// <summary>
/// Query for GetOrder endpoint
/// </summary>
public class GetOrderQuery
{
    [FromRoute(Name = "id")]
    public string OrderId { get; set; } = string.Empty;

    [FromQuery(Name = "date")]
    public DateTime Date { get; set; }
}
