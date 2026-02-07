using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

/// <summary>
/// Command for ExcludeOrderItem endpoint
/// </summary>
public class ExcludeOrderItemCommand
{
    [FromRoute(Name = "orderId")]
    public string OrderId { get; set; } = string.Empty;

    [FromRoute(Name = "itemNumber")]
    public string ItemNumber { get; set; } = string.Empty;
}
