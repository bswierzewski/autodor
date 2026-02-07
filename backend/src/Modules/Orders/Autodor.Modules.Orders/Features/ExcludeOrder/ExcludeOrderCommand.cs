using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Orders.Features.ExcludeOrder;

/// <summary>
/// Command for ExcludeOrder endpoint
/// </summary>
public class ExcludeOrderCommand
{
    [FromRoute(Name = "orderId")]
    public string OrderId { get; set; } = string.Empty;
}
