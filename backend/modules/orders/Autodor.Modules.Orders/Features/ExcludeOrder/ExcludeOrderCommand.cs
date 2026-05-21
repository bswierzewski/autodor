namespace Autodor.Modules.Orders.Features.ExcludeOrder;

/// <summary>
/// Command for exclude/include order endpoint
/// </summary>
public class ExcludeOrderCommand
{
    public string Id { get; set; } = string.Empty;

    public bool Excluded { get; set; }
}
