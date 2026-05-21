using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.GetOrder;

public class GetOrderCommand
{
    [FromRoute(Name = "id")]
    public string Id { get; set; } = string.Empty;

    [FromQuery(Name = "date")]
    public DateTime Date { get; set; }
}