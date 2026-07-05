namespace Autodor.Modules.Orders.Features.ExcludeOrderItem;

/// <summary>
/// Command for exclude/include order item endpoint
/// </summary>
public class ExcludeOrderItemCommand
{
    public string Id { get; set; } = string.Empty;

    public string ItemNumber { get; set; } = string.Empty;

    public bool Excluded { get; set; }
}
