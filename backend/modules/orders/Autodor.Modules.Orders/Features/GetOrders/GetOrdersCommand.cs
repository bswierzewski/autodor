using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.GetOrders;

public class GetOrdersCommand
{
    [FromQuery(Name = "from")]
    public DateTime From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime To { get; set; }
}